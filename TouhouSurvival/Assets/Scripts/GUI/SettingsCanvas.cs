using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class SettingsCanvas : UnchordCanvas
    {
        private Button _btnBack;

        private UnchordCanvas _reservedReturnCanvas;

        protected override void Awake()
        {
            _btnBack = transform.Find("BackButton").GetComponent<Button>();

            _btnBack.onClick.AddListener(OnBackButtonClick);
        }

        public override void Show()
        {
            base.Show();

            s_uiManager.SingleColorCanvas0.LayerBackOf(this);
            s_uiManager.SingleColorCanvas0.Show();
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

        public void ReserveReturnCanvas(UnchordCanvas returnCanvas)
        {
            _reservedReturnCanvas = returnCanvas;
        }

        private void OnBackButtonClick()
        {
            this.Hide();

            Debug.Assert(_reservedReturnCanvas != null);

            _reservedReturnCanvas.Show();
        }
    }
}