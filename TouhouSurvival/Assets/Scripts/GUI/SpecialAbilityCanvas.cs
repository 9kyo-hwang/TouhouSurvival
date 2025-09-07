using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class SpecialAbilityCanvas : UnchordCanvas
    {
        public int SelectedIndex { get; private set; }

        public SelectionButtonSpecial[,] Selections => _selections;

        private Transform _treeRoot;
        private SelectionButtonSpecial[,] _selections;

        private Image _iconSpecial;
        private TooltipEvent _iconTooltipEvent;
        
        public void Clear()
        {
            SelectedIndex = -1;
        }

        protected override void Awake()
        {
            base.Awake();

            _treeRoot = transform.Find("Panel/Tree");
            _selections = new SelectionButtonSpecial[AbilityManager.MAX_SPECIAL_ABILITY_TREE_COUNT, AbilityManager.MAX_SPECIAL_ABILITY_COUNT];
            _iconSpecial = _treeRoot.Find("Special Icon").GetComponent<Image>();
            _iconTooltipEvent = _iconSpecial.transform.GetComponent<TooltipEvent>();
            
            _iconSpecial.sprite = s_gameManager.Player.iconSpecial;
            _iconTooltipEvent.description = s_gameManager.Player.descSpecialAbility;
            
            for (int i = 0; i < AbilityManager.MAX_SPECIAL_ABILITY_TREE_COUNT; ++i)
            {
                for (int j = 0; j < AbilityManager.MAX_SPECIAL_ABILITY_COUNT; ++j)
                {
                    Transform btnTransform = _treeRoot.Find($"Selection {i}{j}");
                    _selections[i, j] = btnTransform.GetComponent<SelectionButtonSpecial>();
                    _selections[i, j].Init(OnSelectionButtonClicked, AbilityManager.MAX_SPECIAL_ABILITY_COUNT * i + j);
                }
            }
        }

        private void OnSelectionButtonClicked(SelectionButtonSpecial button)
        {
            SelectedIndex = button.SelectionIndex;
        }
    }
}