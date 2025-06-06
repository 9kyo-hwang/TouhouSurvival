using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Unchord
{
    public class ShopData
    {
        public int Gold {get; private set;} = 999999;   // 현재 플레이어가 보유한 골드량
        public int InvestedPoints {get; private set;} = 0; // 플레이어가 지금까지 투자한 포인트량
        public int ExchangedPoints {get; private set;} = 0; // 현재 플레이어가 교환한 총 포인트량
        public int InvestablePoints => ExchangedPoints - InvestedPoints;   // 플레이어가 투자할 수 있는 남은 포인트량

        public event System.Action<int> GoldChanged;
        public event System.Action<int, int> PointsChanged;

        private const int BASE_EXCHANGE_RATE = 100;
        private const float EXCHANGE_RATE_INCREASE = 0.1f;
        private int ExcahngeRate => Mathf.RoundToInt(BASE_EXCHANGE_RATE * (1 + (ExchangedPoints * EXCHANGE_RATE_INCREASE)));    // 지금까지 교환한 포인트량에 비례해 증가하는 골드 환전량

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
            InvestedPoints -= totalRefund;
            if(InvestedPoints < 0) InvestedPoints = 0;
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

        private UnchordCanvas _previousCanvas;
        private ShopData _shopData;
        private List<ShopItemView> _itemViews = new List<ShopItemView>();

        protected override void Awake()
        {
            base.Awake();

            _shopData = new ShopData();

            BindButtonEvents();
            BindShopDataEvents();
            InitializeItemViews();
            UpdateGoldDisplay(_shopData.Gold);
            UpdatePointsDisplay(_shopData.InvestablePoints, _shopData.ExchangedPoints);
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
            foreach(Transform child in itemContainer)
            {
                var itemView = child.GetComponent<ShopItemView>();
                if(itemView == null)
                {
                    itemView = child.gameObject.AddComponent<ShopItemView>();
                }

                var itemData = new ShopItemData("Name");
                itemView.Initialize(itemData, _shopData);
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

        public override void Hide()
        {
            base.Hide();
        }
    }
}

