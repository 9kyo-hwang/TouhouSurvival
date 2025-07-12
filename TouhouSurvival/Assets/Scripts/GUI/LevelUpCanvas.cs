using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public class LevelUpCanvas : UnchordCanvas
    {
        public int SelectedIndex { get; private set; } = -1;

        #region Inspector Properties
        public Sprite imgWeaponPanel;
        public Sprite imgPassivePanel;
        public IntegerCounterFontSO levelFont;
        #endregion

        private List<SelectionButton> _btnSelections;
        private int _btnEnabledCount;
        private Transform _selectionPrefab;

        protected override void Awake()
        {
            base.Awake();

            _btnSelections = new List<SelectionButton>(4);
            _selectionPrefab = transform.Find("Selection Buttons/__SelectionButtonPrefab");

            for (int i = 0; i < _btnSelections.Capacity; ++i)
            {
                _btnSelections.Add(new SelectionButton(_selectionPrefab, i, OnSelectionButtonClick));
                _btnSelections[i].Disable();
            }

            _btnEnabledCount = 0;
        }

        public override void Show()
        {
            base.Show();

            SelectedIndex = -1;

            s_uiManager.SingleColorCanvas0.LayerBackOf(this);
            s_uiManager.SingleColorCanvas0.Show();
        }

        public override void Hide()
        {
            base.Hide();

            s_uiManager.SingleColorCanvas0.Hide();
        }

        public void Clear()
        {
            while (_btnEnabledCount > 0)
            {
                --_btnEnabledCount;

                _btnSelections[_btnEnabledCount].Disable();
            }
        }

        public void AddAbility(AbilityComponent ability)
        {
            int i = _btnEnabledCount;
            SelectionButton button = _btnSelections[i];

            if (ability is WeaponComponent)
                button.SetPanel(imgWeaponPanel);
            else if (ability is PassiveComponent)
                button.SetPanel(imgPassivePanel);

            button.SetIcon(ability.DisplayIcon);
            button.SetLevel(ability.CurrentLevel + 1, levelFont);
            button.SetSelectionName(ability.DisplayName);
            button.SetDescription(ability.GetModifierDescription(ability.CurrentLevel + 1));
            button.Enable();

            ++_btnEnabledCount;

            UnityEngine.Debug.Assert(_btnEnabledCount <= _btnSelections.Capacity);
        }

        public void AddNoEntry()
        {
            int i = _btnEnabledCount;
            SelectionButton button = _btnSelections[i];

            button.SetPanel(imgPassivePanel);
            button.SetIcon(null);
            button.SetLevel(0, levelFont);
            button.SetSelectionName("강화 완료");
            button.SetDescription("모든 능력을\n강화했습니다.");
            button.Enable();

            ++_btnEnabledCount;

            UnityEngine.Debug.Assert(_btnEnabledCount <= _btnSelections.Capacity);
        }

        private void OnSelectionButtonClick(int index)
        {
            SelectedIndex = index;
        }
    }
}