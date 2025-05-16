using UnityEngine.UI;

namespace Unchord
{
    public class SpecialAbilityCanvas : UnchordCanvas
    {
        public int SelectedIndex { get; private set; }

        public void Clear()
        {
            SelectedIndex = -1;
        }

        public void SetDescription(int treeIndex, int level, string description)
        {
            UnityEngine.Debug.Assert(treeIndex >= 0);
            UnityEngine.Debug.Assert(level > 0);
        }

        public void SetButtonMode(int treeIndex, int level, int currentLevel)
        {
            UnityEngine.Debug.Assert(treeIndex >= 0);
            UnityEngine.Debug.Assert(level > 0);
            UnityEngine.Debug.Assert(currentLevel >= 0);

            if (level < currentLevel)
            {
                // TODO: 이미 선택된 버튼 구현
            }
            else if (level > currentLevel)
            {
                // TODO: 현재는 선택 불가능하지만 미래에 선택 가능할 수도 있는 버튼 구현
            }
            else
            {
                // TODO: 현재 선택할 수 있는 버튼 구현
            }
        }

        private void OnSelectionButtonClick(Button clickedButton)
        {
            SelectedIndex = clickedButton.transform.parent.GetSiblingIndex();
        }
    }
}