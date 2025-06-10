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

        public bool IsGameStarted { get; private set; }
        public bool IsGamePaused { get; private set; }

        public float AbsolutePlaytime { get; private set; }
        public float ElapsedPlaytime { get; private set; }
        public bool ShouldUpdateElapsedPlaytime { get; set; } = true;

        public int KillCount { get; set; }
        public int EarnedGold { get; set; }
        
        private IPhase _stageTree;

        public List<GameObject> SpawnedEnemies { get; private set; }

        private int _timeStopInterruptCounter = 0;

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

            BlockingEvent.onBlockingEventOccurred += OnBlockEventOccurred;
            BlockingEvent.onBlockingEventHandled += OnBlockEventHandled;

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

            RuntimeState execResult = _stageTree.Update();

            switch (execResult)
            {
                case RuntimeState.Continue:
                    UpdatePlaytime();
                    break;

                case RuntimeState.Pass:
                case RuntimeState.Fail:
                    EndGame(execResult);
                    break;

                case RuntimeState.Halt:
                    _stageTree = null;
                    break;

                default:
                    Debug.Assert(false, "Unknown case handling occured.");
                    break;
            }
        }

        public void StartGame()
        {
            if (IsGameStarted)
                return;

            IsGameStarted = true;

            // TODO: 이름 수정
            StartPhaseRuntimeTree("Phases/Test/New Stage");
            CreatePlayer();
            MainCamera.transform.position = 10.0f * Vector3.back;

            UIManager.Instance.GameCanvas.Show();
        }

        public void PauseGame()
        {
            // TODO: 추후 구현해야 합니다.
        }

        public void ResumeGame()
        {
            // TODO: 추후 구현해야 합니다.
        }

        public void InterruptTimeStop()
        {
            _timeStopInterruptCounter++;
            Time.timeScale = 0.0f;
        }

        public void ReleaseTimeStopInterrupt()
        {
            if (--_timeStopInterruptCounter == 0)
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
            Player instance = GameObject.Instantiate(resource, RuntimeContainer.transform, true);

            instance.name = "Player";
            instance.transform.position = Vector3.zero;

            Player = instance;
            PlayerLoaded?.Invoke(Player);
        }

        public void HaltGame()
        {
            _stageTree.InterruptHalt();
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
            }
        }

        private void EndGame(RuntimeState stageResult)
        {
            UnityEngine.Debug.Assert(stageResult == RuntimeState.Pass || stageResult == RuntimeState.Fail);

            UIManager.Instance.GameCanvas.Hide();

            IsGameStarted = false;

            if(Player != null)
            {
                // TODO: 플레이어 언로드 로직 추가
            }

            GameData.Instance.totalAbsolutePlaytime += AbsolutePlaytime;
            GameData.Instance.totalElapsedPlaytime += ElapsedPlaytime;
            GameData.Instance.totalGamePlayCount += 1;
            GameData.Instance.totalKillCount += KillCount;
            GameData.Instance.gold += EarnedGold;

            if (stageResult == RuntimeState.Pass)
            {
                GameData.Instance.totalGamePlaySuccessCount += 1;
            }
            else
            {
                GameData.Instance.totalGamePlayFailureCount += 1;
            }

            GameData.Instance.Save();
            UIManager.Instance.GameCanvas.Clear();
            UIManager.Instance.GameResultCanvas.Show();
        }

        private void OnBlockEventOccurred()
        {
            _stageTree.Pause();
        }

        private void OnBlockEventHandled()
        {
            _stageTree.Resume();
        }

        private void StartPhaseRuntimeTree(string phaseSoResourcePath)
        {
            if (_stageTree != null)
                return;

            StageDataSO stageSO = Resources.Load(phaseSoResourcePath) as StageDataSO;
            _stageTree = stageSO.CreateRuntime() as IPhase;
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

        public void OnEnemySpawned(object sender, SpawnEventArgs args)
        {
            SpawnedEnemies.Add(args.spawnedInstance);
        }
    }
}