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

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            int samplingCount = 3;
            //List<AbilityComponent> _nextSelections = s_gameManager.Player.SampleAbility(samplingCount);
            Transform selectionParent = null;

            int iCount = _nextSelections.Count;
            int bCount = selectionParent.childCount;
            GameObject buttonPrefab = selectionParent.GetChild(0).gameObject;

            selectionParent.transform.Find("Selection Button (0)");

            for (int i = 0; i < iCount; ++i)
            {
                if (bCount - 1 <= i)
                {
                    GameObject newButton = GameObject.Instantiate(buttonPrefab);
                    newButton.transform.SetParent(selectionParent, false);
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
            _nextSelections[selectionIndex].Level += 1;
            _nextSelections[selectionIndex].SortSiblingIndex();
            this.Hide();
        }
    }
}