using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class SelectionButton
    {
        private int _selectionIndex;

        private Transform _transform;

        private Button _btnSelection;
        private Image _imgPanel;
        private Image _imgIcon;
        private Image _imgLevel;
        private TextMeshProUGUI _txtName;
        private TextMeshProUGUI _txtDescription;

        public SelectionButton(Transform prefab, int index, Action<int> callback)
        {
            _selectionIndex = index;

            _transform = GameObject.Instantiate(prefab);
            _transform.SetParent(prefab.parent, false);

            _btnSelection = _transform.GetComponent<Button>();
            _imgPanel = _transform.GetComponent<Image>();
            _imgIcon = _transform.Find("Icon").GetComponent<Image>();
            _imgLevel = _transform.Find("Level").GetComponent<Image>();
            _txtName = _transform.Find("Name").GetComponent<TextMeshProUGUI>();
            _txtDescription = _transform.Find("Description").GetComponent<TextMeshProUGUI>();

            _btnSelection.onClick.AddListener(() => callback(_selectionIndex));
        }

        public void Enable()
        {
            _transform.gameObject.SetActive(true);
        }

        public void Disable()
        {
            _transform.gameObject.SetActive(false);
        }

        public void SetPanel(Sprite panel)
        {
            _imgPanel.sprite = panel;
        }

        public void SetIcon(Sprite icon)
        {
            _imgIcon.sprite = icon;
        }

        public void SetLevel(int level, IntegerCounterFontSO font)
        {
            _imgLevel.sprite = font.digitSprites[level];
        }

        public void SetSelectionName(string sName)
        {
            _txtName.text = sName;
        }

        public void SetDescription(string desc)
        {
            _txtDescription.text = desc;
        }
    }
}