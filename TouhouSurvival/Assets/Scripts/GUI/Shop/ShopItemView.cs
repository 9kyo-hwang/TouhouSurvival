using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unchord
{
    public class ShopItemView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Image icon;
        private Text titleText;
        private Text levelText;
        private Button upgradeButton;
        private Button downgradeButton;
        private Tooltip tooltip;

        public ShopItemData ItemData { get; private set; }
        private ShopData _shopData;
        private ShopItemDataSO _itemDataSO;

        private void Awake()
        {
            icon            = transform.Find("Image").GetComponent<Image>();
            titleText       = transform.Find("Name").GetComponent<Text>();
            levelText       = transform.Find("Level").GetComponent<Text>();
            upgradeButton   = transform.Find("Up").GetComponent<Button>();
            downgradeButton = transform.Find("Down").GetComponent<Button>();
            tooltip         = GetComponentInChildren<Tooltip>();
        }

        public void Initialize(ShopData shopData, ShopItemDataSO itemDataSO)
        {
            _shopData = shopData;
            _itemDataSO = itemDataSO;

            icon.sprite = _itemDataSO.icon;
            titleText.text = _itemDataSO.title;

            ItemData = new ShopItemData(itemDataSO);
            ItemData.LevelChanged += UpdateDisplay;
            _shopData.PointsChanged += OnPointsChanged;

            UpdateDisplay();
            BindEvents();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltip.Show("Test", Input.mousePosition);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltip.Hide();
        }
        
        private void BindEvents()
        {
            upgradeButton.onClick.AddListener(OnUpgradeClicked);
            downgradeButton.onClick.AddListener(OnDowngradeClicked);
        }
        
        private void UpdateDisplay()
        {
            titleText.text = ItemData.AttributeType;
            levelText.text = $"{ItemData.CurrentLevel}/{ItemData.MaxLevel}";

            upgradeButton.interactable = _shopData.InvestablePoints > 0 && ItemData.CurrentLevel < ItemData.MaxLevel;
            downgradeButton.interactable = ItemData.CurrentLevel > 0;
        }

        private void OnUpgradeClicked()
        {
            if(_shopData.InvestablePoints > 0 && ItemData.TryUpgrade())
            {
                _shopData.InvestPoint();
            }
        }

        private void OnDowngradeClicked()
        {
            if(ItemData.TryDowngrade())
            {
                _shopData.RefundPoint();
            }
        }

        public int ResetLevel()
        {
            int refund = ItemData.CurrentLevel;
            if(refund > 0)
            {
                for(int i=0; i<refund; ++i)
                {
                    _shopData.RefundPoint();
                }

                ItemData.ForceSetLevel(0);
                UpdateDisplay();
            }
            return refund;
        }

        private void OnPointsChanged(int investable, int exchanged)
        {
            UpdateDisplay();
        }

        private void OnDestroy()
        {
            if(ItemData != null) ItemData.LevelChanged -= UpdateDisplay;
            if(_shopData != null) _shopData.PointsChanged -= OnPointsChanged;
        }
    }

    public class ShopItemData
    {
        public string AttributeType { get; private set; }
        private AttributeModifierSet _modifierSet;
        public GameplayAttributeModifier Modifier
        {
            get
            {
                if(_modifierSet != null && CurrentLevel >= 1 && CurrentLevel <= _modifierSet.MaxLevel)
                {
                    return _modifierSet[CurrentLevel];
                }
                
                return null;
            }
        }

        public int CurrentLevel { get; private set; } = 0;
        public int MaxLevel => _modifierSet.MaxLevel;
        public event System.Action LevelChanged;

        public ShopItemData(ShopItemDataSO so)
        {
            _modifierSet = new AttributeModifierSet(so.modifiers);
            Debug.Assert(_modifierSet != null, "AttributeModifierSet is null");
            AttributeType = so.attributeType;
        }
        
        public bool TryUpgrade()
        {
            if (CurrentLevel >= MaxLevel)
            {
                Debug.LogError($"Max level reached for {AttributeType}");
                return false;
            }
            
            CurrentLevel++;
            LevelChanged?.Invoke();
            return true;
        }
        
        public bool TryDowngrade()
        {
            if (CurrentLevel <= 0) return false;
            
            CurrentLevel--;
            LevelChanged?.Invoke();
            return true;
        }

        public void ForceSetLevel(int level)
        {
            CurrentLevel = level;
            LevelChanged?.Invoke();
        }
    }
}