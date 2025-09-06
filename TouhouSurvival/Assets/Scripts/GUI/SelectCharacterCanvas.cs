using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class SelectCharacterCanvas : UnchordCanvas
    {
        private const int MAX_CHARACTER_SLOT_COUNT = 3;

        private Button _btnBack;
        private Button _btnStart;
        private Button _btnLeft;
        private Button _btnRight;
        private CharacterSlot[] _characterSlots;
        private Image _imgPreview;
        private Tooltip _tooltip;

        private int _idxSelected;

        protected override void Awake()
        {
            base.Awake();

            _btnBack = transform.Find("BackButton").GetComponent<Button>();
            _btnStart = transform.Find("StartButton").GetComponent<Button>();
            _btnLeft = transform.Find("CharacterSlots/LeftButton").GetComponent<Button>();
            _btnRight = transform.Find("CharacterSlots/RightButton").GetComponent<Button>();
            _imgPreview = transform.Find("CharacterPreview").GetComponent<Image>();
            _tooltip = GetComponentInChildren<Tooltip>(true);

            base.RegisterTooltipEvent(_tooltip);

            _characterSlots = new CharacterSlot[MAX_CHARACTER_SLOT_COUNT];

            for (int i = 0; i < MAX_CHARACTER_SLOT_COUNT; ++i)
            {
                _characterSlots[i] = transform.Find($"CharacterSlots/Slot ({i})").GetComponent<CharacterSlot>();
            }
            
            _btnBack.onClick.AddListener(OnBackButtonClick);
            _btnStart.onClick.AddListener(OnStartButtonClick);
            _btnLeft.onClick.AddListener(OnLeftButtonClick);
            _btnRight.onClick.AddListener(OnRightButtonClick);
            
            _idxSelected = 0;
        }

        public override void Show()
        {
            base.Show();

            _idxSelected = 0;

            s_gameManager.LoadPlayerPrefabs();

            s_uiManager.SingleColorCanvas0.LayerBackOf(this);
            s_uiManager.SingleColorCanvas0.Show();

            int pCount = s_gameManager.PlayerPrefabs.Length;

            UnityEngine.Debug.Assert(pCount > 0);

            int idxCurrent = _idxSelected;
            int idxPrev = (idxCurrent + pCount - 1) % pCount;
            int idxNext = (idxCurrent + 1) % pCount;

            _idxSelected = idxCurrent;
            s_gameManager.PlayerPrefabIndex = _idxSelected;

            SetIcons(0, idxPrev);
            SetIcons(1, idxCurrent);
            SetIcons(2, idxNext);
            SetPreview(idxCurrent);
        }

        public override void Hide()
        {
            base.Hide();

            s_uiManager.SingleColorCanvas0.Hide();
        }

        public override void UpdateKeyboardInput()
        {
            base.UpdateKeyboardInput();

            if (Input.GetKeyDown(KeyCode.Escape))
                OnBackButtonClick();
        }

        private void OnBackButtonClick()
        {
            this.Hide();
            s_uiManager.LobbyCanvas.Show();
        }

        private void OnStartButtonClick()
        {
            this.Hide();
            s_gameManager.StartGame();
        }

        private void OnLeftButtonClick()
        {
            int pCount = s_gameManager.PlayerPrefabs.Length;

            UnityEngine.Debug.Assert(pCount > 0);

            int idxNext = _idxSelected;
            int idxCurrent = (idxNext + pCount - 1) % pCount;
            int idxPrev = (idxCurrent + pCount - 1) % pCount;

            _idxSelected = idxCurrent;
            s_gameManager.PlayerPrefabIndex = _idxSelected;

            SetIcons(0, idxPrev);
            SetIcons(1, idxCurrent);
            SetIcons(2, idxNext);
            SetPreview(idxCurrent);
        }

        private void OnRightButtonClick()
        {
            int pCount = s_gameManager.PlayerPrefabs.Length;

            UnityEngine.Debug.Assert(pCount > 0);

            int idxPrev = _idxSelected;
            int idxCurrent = (idxPrev + 1) % pCount;
            int idxNext = (idxCurrent + 1) % pCount;

            _idxSelected = idxCurrent;
            s_gameManager.PlayerPrefabIndex = _idxSelected;

            SetIcons(0, idxPrev);
            SetIcons(1, idxCurrent);
            SetIcons(2, idxNext);
            SetPreview(idxCurrent);
        }

        private void SetIcons(int idxSlot, int idxPrefab)
        {
            CharacterSlot slot = _characterSlots[idxSlot];
            Player prefab = s_gameManager.PlayerPrefabs[idxPrefab];

            slot.Show(prefab);
        }

        private void SetPreview(int idxPrefab)
        {
            Player prefab = s_gameManager.PlayerPrefabs[idxPrefab];

            _imgPreview.sprite = prefab.iconPreview;
        }
    }
}