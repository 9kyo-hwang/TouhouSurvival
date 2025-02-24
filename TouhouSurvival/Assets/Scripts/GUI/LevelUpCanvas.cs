using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class LevelUpCanvas : UnchordCanvas
    {
        public int SelectionCount => _shownAbilityCount;
        public int SelectedIndex { get; private set; } = -1;

        private Transform _selectionButtonContainer;
        private Transform _noEntryText;
        private int _shownAbilityCount;

        protected override void Awake()
        {
            base.Awake();

            _selectionButtonContainer = transform.Find("Selection Buttons");
            _noEntryText = _selectionButtonContainer.Find("NoEntry");
            _noEntryText.GetComponent<Button>().onClick.AddListener(OnNoEntryButtonClick);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            SelectedIndex = -1;

            UnityEngine.Debug.Assert(_shownAbilityCount >= 0);
            _noEntryText.gameObject.SetActive(_shownAbilityCount == 0);
        }

        public void Clear()
        {
            for (int i = 1; i <= _shownAbilityCount; ++i)
            {
                string buttonName = GetButtonName(i);
                GameObject button = _selectionButtonContainer.Find(buttonName).gameObject;
                button.SetActive(false);
            }

            _shownAbilityCount = 0;
        }

        public void Add(AbilityComponent ability)
        {
            int index = ++_shownAbilityCount;
            string buttonName = GetButtonName(index);
            Transform buttonTransform = _selectionButtonContainer.Find(buttonName);

            if (buttonTransform == null)
                buttonTransform = CreateNewButton(index);

            buttonTransform.gameObject.SetActive(true);

            this.GetIcon(buttonTransform).sprite = ability.icon;
            this.GetName(buttonTransform).text = ability.name;
            this.GetDescription(buttonTransform).text = "level up description here.";
        }

        private Transform CreateNewButton(int index)
        {
            Transform prefab = _selectionButtonContainer.Find(GetButtonName(0));
            Transform newButton = GameObject.Instantiate(prefab);

            newButton.SetParent(_selectionButtonContainer, false);
            newButton.name = GetButtonName(index);

            Button button = newButton.GetComponent<Button>();
            button.onClick.AddListener(() => OnSelectionButtonClick(newButton));

            return newButton;
        }

        private Image GetIcon(Transform selection)
        {
            return selection.Find("Icon").GetComponent<Image>();
        }

        private TextMeshProUGUI GetName(Transform selection)
        {
            return selection.Find("Name").GetComponent<TextMeshProUGUI>();
        }

        private TextMeshProUGUI GetDescription(Transform selection)
        {
            return selection.Find("Description").GetComponent<TextMeshProUGUI>();
        }

        private void OnNoEntryButtonClick()
        {
            SelectedIndex = 0;
        }

        private void OnSelectionButtonClick(Transform buttonTransform)
        {
            SelectedIndex = GetButtonIndex(buttonTransform);
        }

        private string GetButtonName(int index)
        {
            return $"Selection Button ({index})";
        }

        private int GetButtonIndex(Transform buttonTransform)
        {
            string buttonName = buttonTransform.name;
            string pattern = @"^Selection Button \((\d+)\)$";
            int index;

            Match match = Regex.Match(buttonName, pattern);

            if (!match.Success || !int.TryParse(match.Groups[1].Value, out index))
                return -1;

            UnityEngine.Debug.Assert(index > 0);

            return index - 1;
        }
    }
}