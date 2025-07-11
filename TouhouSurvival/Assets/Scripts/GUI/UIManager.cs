using System.Collections.Generic;

namespace Unchord
{
    // NOTE: UnityEngine.Canvas 컴포넌트가 있는 GameObject에 이 컴포넌트를 부착해야 합니다.
    public class UIManager : UIManagerBase<UIManager>
    {
        #region GUI Canvas Instance Factory Properties
        public MainIllustCanvas MainIllustCanvas => this.GetCanvas<MainIllustCanvas>("GUIs/Canvas/MainIllust", false);
        public LobbyCanvas LobbyCanvas => this.GetCanvas<LobbyCanvas>("GUIs/Canvas/Lobby", true);
        public LoadingCanvas LoadingCanvas => this.GetCanvas<LoadingCanvas>("GUIs/Canvas/Loading");
        public GameResultCanvas GameResultCanvas => this.GetCanvas<GameResultCanvas>("GUIs/Canvas/GameResult");
        public SettingsCanvas SettingsCanvas => this.GetCanvas<SettingsCanvas>("GUIs/Canvas/Settings");
        public GameCanvas GameCanvas => this.GetCanvas<GameCanvas>("GUIs/Canvas/Game");
        public LevelUpCanvas LevelUpCanvas => this.GetCanvas<LevelUpCanvas>("GUIs/Canvas/LevelUp");
        public PauseCanvas PauseCanvas => this.GetCanvas<PauseCanvas>("GUIs/Canvas/Pause");
        public SelectCharacterCanvas SelectCharacterCanvas => this.GetCanvas<SelectCharacterCanvas>("GUIs/Canvas/SelectCharacter");
        public SpecialAbilityCanvas SpecialAbilityCanvas => this.GetCanvas<SpecialAbilityCanvas>("GUIs/Canvas/SpecialAbility");
        public ShopCanvas ShopCanvas => GetCanvas<ShopCanvas>("GUIs/Canvas/Shop");
        public SingleColorCanvas SingleColorCanvas0 => GetCanvas<SingleColorCanvas>("GUIs/Canvas/SingleColorCanvas0");
        #endregion

        public int TopCanvasIndex => _topOfCanvas;
        public UnchordCanvas TopCanvas => _topCanvas;

        private int _topOfCanvas;
        private UnchordCanvas _topCanvas;

        protected override void Awake()
        {
            base.Awake();

            _topOfCanvas = -1;
            _topCanvas = null;
        }

        protected override void Start()
        {
            base.Start();

            this.LobbyCanvas.Show();
        }

        protected override void Update()
        {
            base.Update();

            UpdateTopOfCanvas();
            _topCanvas?.UpdateKeyboardInput();
        }

        private void UpdateTopOfCanvas()
        {
            int i = transform.childCount - 1;

            for (; i >= 0; --i)
            {
                if (transform.GetChild(i).gameObject.activeSelf)
                    break;
            }

            if (_topOfCanvas == i)
                return;

            if (i < 0)
            {
                _topCanvas = null;
                return;
            }

            _topOfCanvas = i;
            _topCanvas = transform.GetChild(i).GetComponent<UnchordCanvas>();
        }

        private T_Canvas GetCanvas<T_Canvas>(string resourcePath, bool showOnInitialLoad = false)
        where T_Canvas : UnchordCanvas
        {
            return base.GetComponentFromTable<T_Canvas>(resourcePath, this.transform, showOnInitialLoad);
        }
    }
}