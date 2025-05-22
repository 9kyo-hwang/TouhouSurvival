using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class SpecialAbilityCanvas : UnchordCanvas
    {
        public int SelectedIndex { get; private set; }

        private Transform _treeRoot;
        
        public enum SelectionState : int
        {
            FutureSelection = 0b00,
            Selectable = 0b10,
            Selected = 0b11,
        }

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

        public void InitButton(int treeIndex, int level, SelectionState selectionMode)
        {
            UnityEngine.Debug.Assert(treeIndex >= 0);
            UnityEngine.Debug.Assert(level > 0);
            
            Transform btnObject = _treeRoot.GetChild(treeIndex).GetChild(level - 1);
            Button button = btnObject.GetComponent<Button>();

            button.interactable = false;
            button.onClick.RemoveAllListeners();

            switch (selectionMode)
            {
                case SelectionState.FutureSelection:
                    // TODO: 현재는 선택 불가능하지만 미래에 선택 가능할 수도 있는 버튼 구현
                    break;

                case SelectionState.Selectable:
                    button.onClick.AddListener(() => OnSelectionButtonClick(button));
                    button.interactable = true;
                    break;

                case SelectionState.Selected:
                    // TODO: 이미 선택된 버튼 구현
                    break;

                default:
                    break;
            }
        }

        private void OnSelectionButtonClick(Button clickedButton)
        {
            SelectedIndex = clickedButton.transform.parent.GetSiblingIndex();
        }
    }
}