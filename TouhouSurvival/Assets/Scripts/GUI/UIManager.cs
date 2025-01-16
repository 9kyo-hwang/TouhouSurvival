using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    // NOTE: UnityEngine.Canvas 컴포넌트가 있는 GameObject에 이 컴포넌트를 부착해야 합니다.
    public class UIManager : Singleton<UIManager>
    {
        private Dictionary<string, UnchordCanvas> _guiObjectTable;

        #region GUI Canvas Instance Factory Properties
        public LobbyCanvas LobbyCanvas => this.GetCanvas<LobbyCanvas>("GUIs/Canvas/Lobby");
        public LoadingCanvas LoadingCanvas => this.GetCanvas<LoadingCanvas>("GUIs/Canvas/Loading");
        public GameResultCanvas GameResultCanvas => this.GetCanvas<GameResultCanvas>("GUIs/Canvas/GameResult");
        public SettingsCanvas SettingsCanvas => this.GetCanvas<SettingsCanvas>("GUIs/Canvas/Settings");
        #endregion

        private void Awake()
        {
            _guiObjectTable = new Dictionary<string, UnchordCanvas>(8);
        }

        private void Start()
        {
            this.LobbyCanvas.Show();
        }

        private T_Canvas GetCanvas<T_Canvas>(string resourcePath, bool showOnInitialLoad = false)
        where T_Canvas : UnchordCanvas
        {
            if (_guiObjectTable.ContainsKey(resourcePath))
                return _guiObjectTable[resourcePath] as T_Canvas;

            T_Canvas resource = Resources.Load<T_Canvas>(resourcePath);
            T_Canvas instance = GameObject.Instantiate<T_Canvas>(resource);

            instance.transform.SetParent(this.transform, false);
            instance.gameObject.SetActive(showOnInitialLoad);
            _guiObjectTable.Add(resourcePath, instance);

            return instance;
        }
    }
}