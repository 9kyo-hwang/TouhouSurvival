using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class LevelUpCanvas : UnchordCanvas
    {
        private List<AbilityComponent> _nextSelections;
        public bool IsButtonClicked { get; private set; }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            IsButtonClicked = false;
            int samplingCount = 3;
            _nextSelections = s_gameManager.Player.SampleAbility(samplingCount);
            Transform selectionParent = null;

            // TODO: selectionParent nullable 수정(추후 UI 구성 완료 후)
            int iCount = _nextSelections.Count;
            int bCount = selectionParent.childCount;
            GameObject buttonPrefab = selectionParent.GetChild(0).gameObject;

            selectionParent.transform.Find("Selection Button (0)");

            for (int i = 0; i < iCount; ++i)
            {
                if (bCount - 1 <= i)
                {
                    GameObject newButton = GameObject.Instantiate(buttonPrefab, selectionParent, false);
                    newButton.name = $"SelectionButtonBase ({i + 1})";
                }

                Transform child = selectionParent.GetChild(i + 1);
                Button button = child.Find("Button").GetComponent<Button>();
                TextMeshProUGUI title = child.Find("Title").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI desc = child.Find("Description").GetComponent<TextMeshProUGUI>();

                int selectionIndex = i;

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnSelectionButtonClick(button, selectionIndex));

                // button.image = _nextSelections[i].icon;
                title.text = _nextSelections[i].GetType().Name;
                // desc.text = _nextSelections[i].description;
                child.gameObject.SetActive(true);
            }

            for (int i = iCount + 1; i < bCount; ++i)
            {
                selectionParent.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void OnSelectionButtonClick(Button button, int selectionIndex)
        {
            IsButtonClicked = true;
            _nextSelections[selectionIndex].Level += 1;
            _nextSelections[selectionIndex].SortSiblingIndex();

            GameCanvas gameCanvas = s_uiManager.GameCanvas;
            
            for (int i = 0; i < s_gameManager.Player.EnabledWeaponCount; ++i)
            {
                AbilityComponent abilityComponent =
                    s_gameManager.Player.WeaponTransform.GetChild(i).GetComponent<AbilityComponent>();
                
                gameCanvas.SetWeaponIcon(i, abilityComponent.icon);
                gameCanvas.SetWeaponLevel(i, abilityComponent.Level);
            }
            
            // TODO: Passive Icon Set Method
            
            // for (int i = 0; i < s_gameManager.Player.EnabledPassiveCount; ++i)
            // {
            //     AbilityComponent abilityComponent =
            //         s_gameManager.Player.PassiveTransform.GetChild(i).GetComponent<AbilityComponent>();
            //     
            //     gameCanvas.SetPassiveIcon(i, abilityComponent.icon);
            //     gameCanvas.SetPassiveLevel(i, abilityComponent.Level);
            // }
            
            this.Hide();
        }
    }
}