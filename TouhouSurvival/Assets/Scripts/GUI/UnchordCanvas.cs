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
    }
}