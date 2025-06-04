using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class LobbyCanvas : UnchordCanvas
    {
        private Button _btnSettings;
        private Button _btnSelectCharacters;
        private Button _btnShop;
        private Button _btnQuit;

        protected override void Awake()
        {
            base.Awake();

            _btnSettings = transform.Find("Navigator/SettingsButton").GetComponent<Button>();
            _btnSelectCharacters = transform.Find("Navigator/SelectCharacterButton").GetComponent<Button>();
            _btnShop = transform.Find("Navigator/ShopButton").GetComponent<Button>();
            _btnQuit = transform.Find("Navigator/QuitButton").GetComponent<Button>();

            _btnSettings.onClick.AddListener(OnSettingsButtonClick);
            _btnSelectCharacters.onClick.AddListener(OnSelectCharactersButtonClick);
            _btnShop.onClick.AddListener(OnClick_ShopButton);
            _btnQuit.onClick.AddListener(OnQuitButtonClick);
        }

        public override void Show()
        {
            base.Show();

            s_uiManager.SettingsCanvas.ReserveReturnCanvas(this);
            s_uiManager.ShopCanvas.ReserveReturnCanvas(this);
            s_uiManager.MainIllustCanvas.Show();
        }

        private void OnSettingsButtonClick()
        {
            this.Hide();
            UIManager.Instance.SettingsCanvas.Show();
        }

        private void OnSelectCharactersButtonClick()
        {
            this.Hide();
            UIManager.Instance.SelectCharacterCanvas.Show();
        }

        private void OnClick_ShopButton()
        {
            this.Hide();
            UIManager.Instance.ShopCanvas.Show();
        }

        private void OnQuitButtonClick()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}