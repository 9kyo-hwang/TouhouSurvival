using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class PauseCanvas : UnchordCanvas
    {
        private Button _mainButton;
        private Button _resumeButton;

        private SpecialSummaryIcon[] _specials;

        protected override void Awake()
        {
            base.Awake();

            _mainButton = transform.Find("Navigators/MainButton").GetComponent<Button>();
            _resumeButton = transform.Find("Navigators/ResumeButton").GetComponent<Button>();

            _resumeButton.onClick.AddListener(s_gameManager.ResumeGame);
            _mainButton.onClick.AddListener(s_gameManager.HaltGame);

            _specials = new SpecialSummaryIcon[7];

            for (int i = 0; i < _specials.Length; ++i)
            {
                _specials[i] = transform.Find($"Special/Icon ({i})").GetComponent<SpecialSummaryIcon>();
            }
        }

        public override void Show()
        {
            base.Show();

            Player player = s_gameManager.Player;
            Transform t0 = player.SpecialTransform0;
            Transform t1 = player.SpecialTransform1;

            // TODO: 어빌리티 매니저를 public property로 열어서 접근을 할까?
            _specials[0].SetIcon(player.iconSpecial);
            _specials[0].SetTooltipDescription(player.descSpecialAbility);
            _specials[0].SetLock(false);

            for (int i = 0; i < 3; ++i)
            {
                _specials[i + 1].Register(t0.GetChild(i).GetComponent<SpecialAbilityComponent>());
                _specials[i + 4].Register(t1.GetChild(i).GetComponent<SpecialAbilityComponent>());
            }

            s_uiManager.SingleColorCanvas0.LayerBackOf(s_uiManager.GameCanvas);
            s_uiManager.SingleColorCanvas0.Show();
        }

        public override void Hide()
        {
            s_uiManager.SingleColorCanvas0.Hide();

            base.Hide();
        }

        public override void UpdateKeyboardInput()
        {
            base.UpdateKeyboardInput();

            if (Input.GetKeyDown(KeyCode.Escape))
                s_gameManager.ResumeGame();
        }
    }
}