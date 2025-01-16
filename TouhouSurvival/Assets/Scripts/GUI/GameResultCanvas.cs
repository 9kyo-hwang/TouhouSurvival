using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class GameResultCanvas : UnchordCanvas
    {
        private TextMeshProUGUI _txtResultText;
        private Button _btnBack;

        private void Awake()
        {
            _txtResultText = transform.Find("Panel/ResultText").GetComponent<TextMeshProUGUI>();
            _btnBack = transform.Find("Panel/BackToMenuButton").GetComponent<Button>();

            _btnBack.onClick.AddListener(OnBackButtonClick);
        }

        public override void Show()
        {
            base.Show();

            GameManager.Properties properties = GameManager.Instance.properties;

            _txtResultText.text = "";
            _txtResultText.text += "Playtime\n";
            _txtResultText.text += GetPlaytimeString(properties.ElapsablePhasePlaytime);
            _txtResultText.text += "\n\n";
            _txtResultText.text += "Character\n";
            _txtResultText.text += "test character name";
            _txtResultText.text += "\n\n";
            _txtResultText.text += "KillCount\n";
            _txtResultText.text += properties.KillCount.ToString();
            _txtResultText.text += "\n\n";
            _txtResultText.text += "Gold\n";
            _txtResultText.text += properties.Gold.ToString();
        }

        private string GetPlaytimeString(float playtime)
        {
            int intPlaytime = Mathf.RoundToInt(playtime);

            int seconds = intPlaytime % 60;
            int minutes = (intPlaytime / 60) % 60;
            int hours = intPlaytime / 3600;

            return string.Format("{0:D02}:{1:D02}:{2:D02}", hours, minutes, seconds);
        }

        private void OnBackButtonClick()
        {
            this.Hide();
            UIManager.Instance.LobbyCanvas.Show();
        }
    }
}