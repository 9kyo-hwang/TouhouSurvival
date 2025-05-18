using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class SpecialAbilityCanvas : UnchordCanvas
    {
        public int SelectedIndex { get; private set; }

        private Transform _treeRoot;

        public void Clear()
        {
            SelectedIndex = -1;
        }

        protected override void Awake()
        {
            base.Awake();

            _treeRoot = transform.Find("SelectionTrees");
        }

        public void SetDescription(int treeIndex, int level, string description)
        {
            UnityEngine.Debug.Assert(treeIndex >= 0);
            UnityEngine.Debug.Assert(level > 0);

            Transform btnObject = _treeRoot.GetChild(treeIndex).GetChild(level - 1);
            TextMeshProUGUI desc = btnObject.Find("Description").GetComponent<TextMeshProUGUI>();

            desc.text = description;
        }

        public void InitButton(int treeIndex, int level, int currentLevel)
        {
            UnityEngine.Debug.Assert(treeIndex >= 0);
            UnityEngine.Debug.Assert(level > 0);
            
            Transform btnObject = _treeRoot.GetChild(treeIndex).GetChild(level - 1);
            Button button = btnObject.GetComponent<Button>();

            button.onClick.RemoveAllListeners();

            ++currentLevel;

            if (button.interactable = (level == currentLevel))
            {
                button.onClick.AddListener(() => OnSelectionButtonClick(button));
                return;
            }
            
            if (level < currentLevel)
            {
                // TODO: 이미 선택된 버튼 구현
            }
            else
            {
                // TODO: 현재는 선택 불가능하지만 미래에 선택 가능할 수도 있는 버튼 구현
            }
        }

        private void OnSelectionButtonClick(Button clickedButton)
        {
            SelectedIndex = clickedButton.transform.parent.GetSiblingIndex();
        }
    }
}