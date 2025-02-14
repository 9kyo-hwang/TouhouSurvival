using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unchord
{
    public class GameManager : Singleton<GameManager>
    {
        private const int INITIAL_EVENT_CAPACITY = 16;
        private const float BLOCKING_EVENT_HANDLING_COOLTIME = 1.0f;
        private const string PLAYABLE_CHARACTER_RESOURCES_DIRECTORY = "Prefabs/PlayableCharacters/";

        public Player[] PlayerPrefabs { get; private set; }
        public int PlayerPrefabIndex { get; set; } = -1;

        public bool IsGameStarted { get; private set; }

        public float AbsolutePlaytime { get; private set; }
        public float ElapsedPlaytime { get; private set; }
        public bool ShouldUpdateElapsedPlaytime { get; set; }

        public int KillCount { get; set; }
        public int EarnedGold { get; set; }

        private bool _blockingEventFlag;
        private float _blockingEventHandlingCooltimeLeft;
        private Queue<IEnumerator> _blockingEventHandlers;

        private PhaseRuntimeState _phaseExecutionResult;

        private PhaseRuntime _phaseRuntimeTree;

        private int _timeStopInterruptCounter = 0;

        public Camera MainCamera { get; private set; }
        private Player _player;
        public Player Player => _player;

        public Transform RuntimeContainer { get; private set; }
        
        private void TraceCamera()
        {
            if (!_player) return;

            float limit = 2.0f;
            float traceSpeed = 5f;

            Vector2 source = MainCamera.transform.position;
            Vector2 destination = _player.transform.position;

            Vector2 next = Vector2.Lerp(source, destination, traceSpeed * Time.deltaTime);

            if (Vector2.Distance(next, destination) > limit)
            {
                next = (next - destination).normalized * limit + destination;
            }

            Vector3 camPosition = new Vector3(next.x, next.y, MainCamera.transform.position.z);
            MainCamera.transform.position = camPosition;
        }

        private void LateUpdate()
        {
            TraceCamera();
        }

        private void Awake()
        {
            _blockingEventHandlers = new Queue<IEnumerator>(INITIAL_EVENT_CAPACITY);
            MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

            RuntimeContainer = new GameObject("@Runtime Container").transform;

            DontDestroyOnLoad(RuntimeContainer);
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            if (IsGameStarted)
            {
                HandleEvents();
                UpdatePhaseRuntimeTree();
                UpdatePlaytime();
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
            Player instance = GameObject.Instantiate(resource);
            instance.name = "Player";
            instance.transform.parent = RuntimeContainer.transform;
            instance.transform.position = Vector3.zero;
            
            // TEMP
            // _player = instance.GetComponent<Player>();
            _player = instance;
        }

        public void HaltGame()
        {
            _phaseExecutionResult = PhaseRuntimeState.Fail;
            _phaseRuntimeTree = null;
            EndGame();
        }

        public void CleanupGame()
        {
            for (int i = RuntimeContainer.childCount - 1; i >= 0; --i)
            {
                Destroy(RuntimeContainer.GetChild(i).gameObject);
            }

            ClearPlaytime();
        }

        private void EndGame()
        {
            IsGameStarted = false;

            UIManager.Instance.GameResultCanvas.Show();
        }

        public void PublishEvent(IEnumerator eventHandler)
        {
            _blockingEventHandlers.Enqueue(eventHandler);
        }

        private void HandleEvents()
        {
            if (_blockingEventFlag)
                return;
            else if (_blockingEventHandlingCooltimeLeft > 0.0f)
                _blockingEventHandlingCooltimeLeft -= Time.deltaTime;
            else if (_blockingEventHandlers.Count > 0)
                StartCoroutine(HandleBlockingEvent(_blockingEventHandlers.Dequeue()));
        }

        private IEnumerator HandleBlockingEvent(IEnumerator eventHandler)
        {
            _phaseRuntimeTree.Pause();
            _blockingEventFlag = true;
            yield return StartCoroutine(eventHandler);
            _blockingEventHandlingCooltimeLeft = BLOCKING_EVENT_HANDLING_COOLTIME;
            _blockingEventFlag = false;
            _phaseRuntimeTree.Resume();
        }

        private void StartPhaseRuntimeTree(string phaseSoResourcePath)
        {
            if (_phaseRuntimeTree != null)
                return;

            PhaseSO phaseSO = Resources.Load(phaseSoResourcePath) as PhaseSO;

            _phaseRuntimeTree = PhaseRuntimeFactory.CreateRuntime(phaseSO);
            _phaseRuntimeTree.Start();
        }

        private void UpdatePhaseRuntimeTree()
        {
            if (_phaseRuntimeTree == null)
                return;

            _phaseRuntimeTree.Update();

            PhaseRuntimeState phaseRuntimeState = _phaseRuntimeTree.CheckPhaseRuntimeState();

            switch (phaseRuntimeState)
            {
                case PhaseRuntimeState.Continue:
                    // NOTE: This case has intentionally no operation.
                    break;

                case PhaseRuntimeState.Pass:
                    _phaseRuntimeTree.End();
                    _phaseExecutionResult = phaseRuntimeState;

                    if (_phaseRuntimeTree.TrySearchNextRuntime())
                        _phaseRuntimeTree.Start();
                    else
                    {
                        _phaseRuntimeTree = null;
                        UIManager.Instance.GameCanvas.Hide();
                        EndGame();
                    }
                    break;

                case PhaseRuntimeState.Fail:
                    _phaseRuntimeTree.End();
                    _phaseRuntimeTree = null;
                    _phaseExecutionResult = phaseRuntimeState;
                    UIManager.Instance.GameCanvas.Hide();
                    EndGame();
                    break;

                default:
                    Debug.Assert(false, "Unknown case handling occured.");
                    break;
            }
        }

        private void UpdatePlaytime()
        {
            AbsolutePlaytime += Time.deltaTime;

            if (ShouldUpdateElapsedPlaytime)
            {
                ElapsedPlaytime += Time.deltaTime;
                UIManager.Instance.GameCanvas.SetTimer((int)ElapsedPlaytime);
            }
        }

        private void ClearPlaytime()
        {
            AbsolutePlaytime = 0.0f;
            ElapsedPlaytime = 0.0f;
        }
    }
}