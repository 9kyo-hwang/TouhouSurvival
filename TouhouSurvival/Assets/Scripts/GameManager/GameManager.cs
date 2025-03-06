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

        public float AbsolutePlaytime { get; private set; }
        public float ElapsedPlaytime { get; private set; }
        public bool ShouldUpdateElapsedPlaytime { get; set; }

        public int KillCount { get; set; }
        public int EarnedGold { get; set; }

        private PhaseRuntimeState _phaseExecutionResult;

        private PhaseRuntime _phaseRuntimeTree;

        private int _timeStopInterruptCounter = 0;

        public BlockingEventHandler BlockingEvent { get; private set; }
        public Camera MainCamera { get; private set; }
        public Player Player { get; private set; }

        public Transform RuntimeContainer { get; private set; }
        
        private void Awake()
        {
            BlockingEvent = GetComponent<BlockingEventHandler>();
            MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

            RuntimeContainer = new GameObject("@Runtime Container").transform;

            BlockingEvent.onBlockingEventOccurred += OnBlockEventOccurred;
            BlockingEvent.onBlockingEventHandled += OnBlockEventHandled;

            DontDestroyOnLoad(RuntimeContainer);
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            if (IsGameStarted)
            {
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
            Player instance = GameObject.Instantiate(resource, RuntimeContainer.transform, true);

            instance.name = "Player";
            instance.transform.position = Vector3.zero;

            Player = instance;
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

            GameData.Instance.totalAbsolutePlaytime += AbsolutePlaytime;
            GameData.Instance.totalElapsedPlaytime += ElapsedPlaytime;
            GameData.Instance.totalGamePlayCount += 1;

            if (_phaseExecutionResult == PhaseRuntimeState.Pass)
            {
                GameData.Instance.totalGamePlaySuccessCount += 1;
            }
            else
            {
                UnityEngine.Debug.Assert(_phaseExecutionResult == PhaseRuntimeState.Fail);
                GameData.Instance.totalGamePlayFailureCount += 1;
            }

            GameData.Instance.Save();
            UIManager.Instance.GameResultCanvas.Show();
        }

        private void OnBlockEventOccurred()
        {
            _phaseRuntimeTree.Pause();
        }

        private void OnBlockEventHandled()
        {
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