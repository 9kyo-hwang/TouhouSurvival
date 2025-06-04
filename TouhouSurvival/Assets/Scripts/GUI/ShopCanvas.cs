using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    internal class PointPanel
    {
        private Text _quantity;
        private Button _exchange;
        private Button _reset;

        public PointPanel(Transform panel)
        {
            _quantity = panel.Find("QuantityText").GetComponent<Text>();
            _exchange = panel.Find("ExchangeButton").GetComponent<Button>();
            _reset = panel.Find("ResetButton").GetComponent<Button>();
            
            _exchange.onClick.AddListener(OnClick_Exchange);
            _reset.onClick.AddListener(OnClick_Reset);
        }

        private void OnClick_Exchange()
        {
            
        }

        private void OnClick_Reset()
        {
            
        }
    }

    internal class GoldPanel
    {
        private Image _image;
        private Text _text;

        public GoldPanel(Transform panel)
        {
            _image = panel.Find("Image").GetComponent<Image>();
            _text = panel.Find("Text").GetComponent<Text>();
        }
        
        
    }
    
    public class ShopCanvas : UnchordCanvas
    {
        private Button _btnBack;
        private PointPanel _panelPoint;
        private GoldPanel _panelGold;

        private UnchordCanvas _prevCanvas;

        protected override void Awake()
        {
            base.Awake();

            _btnBack = transform.Find("BackgroundPanel/ExitButton").GetComponent<Button>();
            _panelPoint = new PointPanel(transform.Find("BackgroundPanel/PointPanel"));
            _panelGold = new GoldPanel(transform.Find("BackgroundPanel/GoldPanel"));
            
            _btnBack.onClick.AddListener(OnClick_BackButton);
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void ReserveReturnCanvas(UnchordCanvas canvas)
        {
            _prevCanvas = canvas;
        }

        private void OnClick_BackButton()
        {
            Hide();
            Debug.Assert(_prevCanvas is not null);
            _prevCanvas.Show();
        }
    }
}

