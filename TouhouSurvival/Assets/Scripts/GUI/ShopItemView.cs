using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace Unchord
{
    public class ShopItemView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text titleText;
        [SerializeField] private Text levelText;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button downgradeButton;
        [SerializeField] private string xlsxPath;

        private ShopItemData _itemData;
        private ShopData _shopData;

        public void Initialize(ShopData shopData)
        {
            _shopData = shopData;
            _itemData = new ShopItemData(xlsxPath);

            _itemData.LevelChanged += UpdateDisplay;
            _shopData.PointsChanged += OnPointsChanged;

            UpdateDisplay();
            BindEvents();
        }
        
        private void BindEvents()
        {
            upgradeButton.onClick.AddListener(OnUpgradeClicked);
            downgradeButton.onClick.AddListener(OnDowngradeClicked);
        }
        
        private void UpdateDisplay()
        {
            titleText.text = _itemData.AttributeType;
            levelText.text = $"{_itemData.CurrentLevel}/{_itemData.MaxLevel}";

            upgradeButton.interactable = _shopData.InvestablePoints > 0 && _itemData.CurrentLevel < _itemData.MaxLevel;
            downgradeButton.interactable = _itemData.CurrentLevel > 0;
        }

        private void OnUpgradeClicked()
        {
            if(_shopData.InvestablePoints > 0 && _itemData.TryUpgrade())
            {
                _shopData.InvestPoint();
            }
        }

        private void OnDowngradeClicked()
        {
            if(_itemData.TryDowngrade())
            {
                _shopData.RefundPoint();
            }
        }

        public int ResetLevel()
        {
            int refund = _itemData.CurrentLevel;
            if(refund > 0)
            {
                for(int i=0; i<refund; ++i)
                {
                    _shopData.RefundPoint();
                }

                _itemData.ForceSetLevel(0);
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
            if(_itemData != null) _itemData.LevelChanged -= UpdateDisplay;
            if(_shopData != null) _shopData.PointsChanged -= OnPointsChanged;
        }
    }

    public class ShopItemData
    {
        public string AttributeType { get; private set; } 
        public int CurrentLevel { get; private set; } = 0;
        public int MaxLevel => _attributeModifier.MaxLevel;
        public event System.Action LevelChanged;
        private AttributeModifierSet _attributeModifier;

        public ShopItemData(string xlsxPath)
        {
            string[] csvPaths = AttributeUtility.ConvertXlsxToCsv(xlsxPath);
            _attributeModifier = AttributeModifierSet.LoadFromFile(csvPaths[1]);
            
            Debug.Assert(_attributeModifier != null, "AttributeModifierSet is null");
            string fileName = Path.GetFileNameWithoutExtension(csvPaths[1]);
            AttributeType = fileName.Split('+')[0];
        }
        
        public bool TryUpgrade()
        {
            if (CurrentLevel >= MaxLevel) return false;
            
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