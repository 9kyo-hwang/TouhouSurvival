using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unchord
{
    public class GameManager : Singleton<GameManager>
    {
        private const string PLAYABLE_CHARACTER_RESOURCES_DIRECTORY = "Prefabs/PlayableCharacters/";

        public Player[] PlayerPrefabs { get; private set; }
        public int PlayerPrefabIndex { get; set; } = -1;

        public bool IsGameStarted
        {
            get
            {
                bool started = true;

                started &= _isGameStarted;
                started &= this.Player.IsStarted;

                return started;
            }
        }

        public bool IsGamePaused { get; private set; }

        public float AbsolutePlaytime { get; private set; }
        public float ElapsedPlaytime { get; private set; }
        public bool ShouldUpdateElapsedPlaytime { get; set; } = true;

        public int KillCount { get; set; }
        public int EarnedGold { get; set; }

        public int ResurrectedCount { get; set; }
        public int ResurrectCountMax { get; set; }

        public bool IsPlayerDead => (this.Player.CurrentHealth <= 0.0f);
        public bool IsRuntimeBlocked => (_execBlockingCounter > 0);
        public PhaseRuntimeCommons PhaseRuntimeCommonData { get; private set; }

        private bool _isGameStarted;

        private IPhase _stageTree;

        public List<GameObject> SpawnedEnemies { get; private set; }

        private int _timeStopInterruptCounter = 0;
        private int _execBlockingCounter = 0;

        public BlockingEventHandler BlockingEvent { get; private set; }
        public Camera MainCamera { get; private set; }
        public Player Player { get; private set; }

        public Transform RuntimeContainer { get; private set; }
        public Transform ProjectileContainer { get; private set; }

        public System.Action<Player> PlayerLoaded;
        public System.Action<Player> PlayerUnloaded;
        
        private void Awake()
        {
            BlockingEvent = GetComponent<BlockingEventHandler>();
            MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

            RuntimeContainer = new GameObject("@Runtime Container").transform;
            ProjectileContainer = new GameObject("@Projectile Container").transform;

            ProjectileContainer.SetParent(RuntimeContainer);

            SpawnedEnemies = new List<GameObject>(1024);

            DontDestroyOnLoad(RuntimeContainer);
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            if (_stageTree == null)
                return;

            if (_execBlockingCounter > 0)
                return;

            RuntimeState execResult = _stageTree.Update();

            switch (execResult)
            {
                case RuntimeState.Continue:
                    UpdatePlaytime();
                    break;

                case RuntimeState.Resurrect:
                    this.BlockingEvent.Publish(OnResurrectCoroutine());
                    break;

                case RuntimeState.Pass:
                case RuntimeState.Fail:
                    this.BlockingEvent.Publish(OnGameEndCoroutine(execResult));
                    break;

                case RuntimeState.Halt:
                    this.BlockingEvent.Publish(OnGameEndCoroutine(RuntimeState.Fail));
                    break;

                default:
                    Debug.Assert(false, "Unknown case handling occured.");
                    break;
            }
        }

        public void StartGame()
        {
            if (_isGameStarted)
                return;

            ResurrectedCount = 0;
            ResurrectCountMax = 0;

            // TODO: 이름 수정
            CreatePlayer();
            StartPhaseRuntimeTree("Phases/Test/New Stage");
            MainCamera.transform.position = 10.0f * Vector3.back;

            _isGameStarted = true;

            UIManager.Instance.GameCanvas.Show();
        }

        public void PauseGame()
        {
            if (IsGamePaused)
                return;

            IsGamePaused = true;
            _stageTree.Pause();
            InterruptTimeStop();

            UIManager ui = UIManager.Instance;

            ui.PauseCanvas.Show();
        }

        public void ResumeGame()
        {
            if (!IsGamePaused)
                return;

            IsGamePaused = false;
            _stageTree.Resume();
            ReleaseTimeStopInterrupt();

            UIManager ui = UIManager.Instance;

            ui.PauseCanvas.Hide();
        }

        public void HaltGame()
        {
            ResumeGame();
            _stageTree.InterruptHalt();
        }

        public void InterruptTimeStop()
        {
            if (++_timeStopInterruptCounter > 0)
                Time.timeScale = 0.0f;
        }

        public void ReleaseTimeStopInterrupt()
        {
            if (--_timeStopInterruptCounter <= 0)
                Time.timeScale = 1.0f;
        }

        public void LoadPlayerPrefabs()
        {
            PlayerPrefabs = Resources.LoadAll<Player>(PLAYABLE_CHARACTER_RESOURCES_DIRECTORY);
            PlayerPrefabIndex = -1;
        }

        private void CreatePlayer()
        {
            Player resource = PlayerPrefabs[PlayerPrefabIndex];
            Player instance = GameObject.Instantiate(resource, null, true);

            instance.name = "Player";
            instance.transform.position = Vector3.zero;

            Player = instance;
            PlayerLoaded?.Invoke(Player);
        }

        public void CleanupGame()
        {
            for (int i = RuntimeContainer.childCount - 1; i >= 0; --i)
            {
                Destroy(RuntimeContainer.GetChild(i).gameObject);
            }

            KillCount = 0;
            EarnedGold = 0;
            AbsolutePlaytime = 0.0f;
            ElapsedPlaytime = 0.0f;
            SpawnedEnemies.Clear();

            if(Player != null)
            {
                // TODO: 플레이어 언로드 로직 추가
                Destroy(Player.gameObject);
            }

            _stageTree = null;
        }

        private void EndGame(RuntimeState stageResult)
        {
            UnityEngine.Debug.Assert(stageResult == RuntimeState.Pass || stageResult == RuntimeState.Fail);

            _isGameStarted = false;

            if(Player != null)
            {
                // TODO: 플레이어 언로드 로직 추가
            }

            GameData.Instance.totalAbsolutePlaytime += AbsolutePlaytime;
            GameData.Instance.totalElapsedPlaytime += ElapsedPlaytime;
            GameData.Instance.totalGamePlayCount += 1;
            GameData.Instance.totalKillCount += KillCount;
            GameData.Instance.Gold += EarnedGold;

            if (stageResult == RuntimeState.Pass)
            {
                GameData.Instance.totalGamePlaySuccessCount += 1;
            }
            else
            {
                GameData.Instance.totalGamePlayFailureCount += 1;
            }

            GameData.Instance.Save();

            UIManager ui = UIManager.Instance;

            // TODO:
            // GameResultCanvas의 디자인이 완성된 후 값을 이 곳에서 UI로 넣어줍니다.
            // 이후 GameResultCanvas에서 GameManager로 접근해 UI에 값을 넣는 코드를 제거합니다.
            ui.GameResultCanvas.Show();
        }

        private void StartPhaseRuntimeTree(string phaseSoResourcePath)
        {
            if (_stageTree != null)
                return;

            PhaseRuntimeCommonData = new PhaseRuntimeCommons(this);

            StageDataSO stageSO = Resources.Load(phaseSoResourcePath) as StageDataSO;
            _stageTree = stageSO.CreateRuntime(PhaseRuntimeCommonData) as IPhase;
        }

        private void UpdatePlaytime()
        {
            if (IsGamePaused)
                return;

            AbsolutePlaytime += Time.deltaTime;

            if (ShouldUpdateElapsedPlaytime)
            {
                ElapsedPlaytime += Time.deltaTime;
                UIManager.Instance.GameCanvas.SetTimer((int)ElapsedPlaytime);
            }
        }

        private IEnumerator OnResurrectCoroutine()
        {
            ++_execBlockingCounter;
            this.Player.Animator.updateMode = AnimatorUpdateMode.UnscaledTime;

            yield return new WaitUntil(this.Player.IsDeadAnimationEnd);
            yield return new WaitForSecondsRealtime(1.0f);

            --_execBlockingCounter;
            ++ResurrectedCount;

            _stageTree.InterruptResurrect();
            this.Player.Animator.updateMode = AnimatorUpdateMode.Normal;
            this.Player.Resurrect();
        }

        private IEnumerator OnGameEndCoroutine(RuntimeState result)
        {
            ++_execBlockingCounter;
            this.Player.Animator.updateMode = AnimatorUpdateMode.UnscaledTime;

            EndGame(result);

            yield return new WaitUntil(() => UIManager.Instance.GameResultCanvas.IsResultButtonClicked);

            --_execBlockingCounter;
            this.Player.Animator.updateMode = AnimatorUpdateMode.Normal;

            CleanupGame();

            // Return To Menu
            UIManager.Instance.GameCanvas.Hide();
            UIManager.Instance.GameCanvas.Clear();
            UIManager.Instance.GameResultCanvas.Hide();
            UIManager.Instance.LobbyCanvas.Show();
        }

        public void OnEnemySpawned(object sender, SpawnEventArgs args)
        {
            SpawnedEnemies.Add(args.spawnedInstance);
        }
    }
}