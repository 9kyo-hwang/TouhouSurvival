using UnityEngine;

namespace Unchord
{
    public abstract class UnchordCanvas : MonoBehaviour
    {
        protected static GameManager s_gameManager;
        protected static UIManager s_uiManager;
        protected static WorldUIManager s_wuiManager;

        protected virtual void Awake()
        {
            if (s_gameManager == null)
                s_gameManager = GameManager.Instance;

            if (s_uiManager == null)
                s_uiManager = UIManager.Instance;

            if (s_wuiManager == null)
                s_wuiManager = WorldUIManager.Instance;
        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public void LayerBackOf(UnchordCanvas canvas)
        {
            UnityEngine.Debug.Assert(this.transform.parent == canvas.transform.parent);

            int i = this.transform.GetSiblingIndex();
            int j = canvas.transform.GetSiblingIndex();

            if (j <= i)
                transform.SetSiblingIndex(j);
            else
                transform.SetSiblingIndex(j - 1);
        }

        public void LayerFrontOf(UnchordCanvas canvas)
        {
            UnityEngine.Debug.Assert(this.transform.parent == canvas.transform.parent);

            int i = this.transform.GetSiblingIndex();
            int j = canvas.transform.GetSiblingIndex();

            if (j < i)
                transform.SetSiblingIndex(j + 1);
            else
                transform.SetSiblingIndex(j);
        }

        public virtual void UpdateKeyboardInput()
        {

        }
    }
}