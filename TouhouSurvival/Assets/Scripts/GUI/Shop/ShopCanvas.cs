using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Unchord
{
    public class ShopData
    {
        public int Gold // 현재 플레이어가 보유한 골드량
        {
            get => GameData.Instance.Gold;
            private set => GameData.Instance.Gold = value;
        }

        // 플레이어가 지금까지 투자한 포인트량
        public int InvestedPoints {get; private set;} = 0; 
        // 현재 플레이어가 교환한 총 포인트량
        public int ExchangedPoints {get; private set;} = 0; 
        // 플레이어가 투자할 수 있는 남은 포인트량
        public int InvestablePoints => ExchangedPoints - InvestedPoints; 

        public event System.Action<int> GoldChanged;
        public event System.Action<int, int> PointsChanged;

        private const int BASE_EXCHANGE_RATE = 100;
        private const float EXCHANGE_RATE_INCREASE = 0.1f;
        private int ExcahngeRate => Mathf.RoundToInt(BASE_EXCHANGE_RATE * (1 + (ExchangedPoints * EXCHANGE_RATE_INCREASE)));    // 지금까지 교환한 포인트량에 비례해 증가하는 골드 환전량

        public void Load(ShopSaveData shopSaveData)
        {
            Gold = shopSaveData.gold;
            InvestedPoints = shopSaveData.investedPoints;
            ExchangedPoints = shopSaveData.exchangedPoints;
        }

        public bool TryExchangePoints(int points = 1)
        {
            int requiredGold = points * ExcahngeRate;
            if(Gold < requiredGold)
            {
                return false;
            }

            Gold -= requiredGold;
            ExchangedPoints += points;

            GoldChanged?.Invoke(Gold);
            PointsChanged?.Invoke(InvestablePoints, ExchangedPoints);

            return true;
        }

        public void InvestPoint()
        {
            InvestedPoints++;
            PointsChanged?.Invoke(InvestablePoints, ExchangedPoints);
        }

        public void RefundPoint()
        {
            InvestedPoints--;
            PointsChanged?.Invoke(InvestablePoints, ExchangedPoints);
        }

        public void ResetInvestedPoints(int totalRefund)
        {
            InvestedPoints = Mathf.Max(InvestedPoints - totalRefund, 0);
            PointsChanged?.Invoke(InvestablePoints, ExchangedPoints);
        }
    }

    public class ShopCanvas : UnchordCanvas
    {
        [SerializeField] private Text goldText;
        [SerializeField] private Text pointsText;
        [SerializeField] private Button backButton;
        [SerializeField] private Button exchangeButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Transform itemContainer;
        [SerializeField] private GameObject itemViewTemplate;
        [SerializeField] private ShopItemShelfSO itemShelfSO;

        private UnchordCanvas _previousCanvas;
        private ShopData _shopData = new ShopData();
        private List<ShopItemView> _itemViews = new List<ShopItemView>();

        protected override void Awake()
        {
            base.Awake();

            BindButtonEvents();
            BindShopDataEvents();
            InitializeItemViews();
        }

        private void Start()
        {
            s_gameManager.PlayerLoaded += OnPlayerLoaded;
            s_gameManager.PlayerUnloaded += OnPlayerUnloaded;
        }

        protected override void OnEnable()
        {
            // TODO: Load Gold, Stat Points and Invested Points from GameData 
            ShopSaveData shopSaveData = GameData.Instance.shopSaveData;

            _shopData.Load(shopSaveData);
            foreach(var itemView in _itemViews)
            {
                if(shopSaveData.TryGetItemLevel(itemView.ItemData.AttributeType, out int level))
                {
                    itemView.ItemData.ForceSetLevel(level);
                }
            }

            UpdateGoldDisplay(shopSaveData.gold);
            UpdatePointsDisplay(shopSaveData.investedPoints, shopSaveData.exchangedPoints);
        }

        protected override void OnDisable()
        {
            // TODO: Save Gold, Stat Points and Invested Points to GameData
            ShopSaveData shopSaveData = GameData.Instance.shopSaveData;

            shopSaveData.investedPoints = _shopData.InvestedPoints;
            shopSaveData.exchangedPoints = _shopData.ExchangedPoints;

            shopSaveData.ClearItemLevels();
            foreach(ShopItemView itemView in _itemViews)
            {
                shopSaveData.Update(itemView.ItemData.AttributeType, itemView.ItemData.CurrentLevel);
            }

            GameData.Instance.Save();
        }

        public override void UpdateKeyboardInput()
        {
            base.UpdateKeyboardInput();

            if (Input.GetKeyDown(KeyCode.Escape))
                OnBackClicked();
        }

        private void OnPlayerLoaded(Player player)
        {
            foreach(var itemView in _itemViews)
            {
                var modifier = itemView.ItemData.Modifier;
                if(modifier != null)
                {
                    player.AttributeBase.ApplyModifiers(modifier);
                }
            }
        }

        private void OnPlayerUnloaded(Player player)
        {

        }

        private void BindButtonEvents()
        {
            backButton.onClick.AddListener(OnBackClicked);
            exchangeButton.onClick.AddListener(OnExchangeClicked);
            resetButton.onClick.AddListener(OnResetClicked);
        }

        private void BindShopDataEvents()
        {
            _shopData.GoldChanged += UpdateGoldDisplay;
            _shopData.PointsChanged += UpdatePointsDisplay;
        }

        private void InitializeItemViews()
        {
            _itemViews.Clear();
            itemViewTemplate.SetActive(false);

            if(itemShelfSO == null || itemShelfSO.itemDataSOs == null)
            {
                Debug.LogError("ItemShelf is not assigned or empty in ShopCanvas");
                return;
            }

            foreach (ShopItemDataSO itemDataSO in itemShelfSO.itemDataSOs)
            {
                if(itemDataSO == null)
                {
                    continue;
                }

                GameObject itemViewGO = Instantiate(itemViewTemplate, itemContainer);
                itemViewGO.SetActive(true);

                ShopItemView itemView = itemViewGO.GetComponent<ShopItemView>();
                itemView.Initialize(_shopData, itemDataSO);
                _itemViews.Add(itemView);
            }
        }

        private void UpdateGoldDisplay(int gold)
        {
            goldText.text = gold.ToString("D6");
        }

        private void UpdatePointsDisplay(int investable, int exchanged)
        {
            pointsText.text = $"{investable}/{exchanged}";
        }

        private void OnExchangeClicked()
        {
            if(_shopData.TryExchangePoints())
            {
                // UI는 이벤트로 자동 갱신됨
            }
        }

        private void OnResetClicked()
        {
            int totalRefund = 0;
            foreach(var itemView in _itemViews)
            {
                totalRefund += itemView.ResetLevel();
            }

            if(totalRefund > 0)
            {
                _shopData.ResetInvestedPoints(totalRefund);
            }

            // Point가 변경되면 ItemView에서 UpdateDisplay가 자동으로 호출됨

            _shopData.ResetInvestedPoints(totalRefund);
        }

        private void OnBackClicked()
        {
            this.Hide();

            Debug.Assert(_previousCanvas != null);

            _previousCanvas.Show();
        }

        public void ReserveReturnCanvas(UnchordCanvas returnCanvas)
        {
            _previousCanvas = returnCanvas;
        }
    }
}

