using System.Collections.Generic;

namespace Unchord
{
    // NOTE: UnityEngine.Canvas 컴포넌트가 있는 GameObject에 이 컴포넌트를 부착해야 합니다.
    public class UIManager : UIManagerBase<UIManager>
    {
        #region GUI Canvas Instance Factory Properties
        public LobbyCanvas LobbyCanvas => this.GetCanvas<LobbyCanvas>("GUIs/Canvas/Lobby", true);
        public LoadingCanvas LoadingCanvas => this.GetCanvas<LoadingCanvas>("GUIs/Canvas/Loading");
        public GameResultCanvas GameResultCanvas => this.GetCanvas<GameResultCanvas>("GUIs/Canvas/GameResult");
        public SettingsCanvas SettingsCanvas => this.GetCanvas<SettingsCanvas>("GUIs/Canvas/Settings");
        public GameCanvas GameCanvas => this.GetCanvas<GameCanvas>("GUIs/Canvas/Game");
        public LevelUpCanvas LevelUpCanvas => this.GetCanvas<LevelUpCanvas>("GUIs/Canvas/LevelUp");
        public PauseCanvas PauseCanvas => this.GetCanvas<PauseCanvas>("GUIs/Canvas/Pause");
        public SelectCharacterCanvas SelectCharacterCanvas => this.GetCanvas<SelectCharacterCanvas>("GUIs/Canvas/SelectCharacter");
        #endregion

        protected override void Start()
        {
            base.Start();

            this.LobbyCanvas.Show();
        }

        private T_Canvas GetCanvas<T_Canvas>(string resourcePath, bool showOnInitialLoad = false)
        where T_Canvas : UnchordCanvas
        {
            return base.GetComponentFromTable<T_Canvas>(resourcePath, this.transform, showOnInitialLoad);
        }
    }
}