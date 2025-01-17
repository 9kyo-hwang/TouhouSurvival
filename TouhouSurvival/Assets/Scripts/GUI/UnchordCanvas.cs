using UnityEngine;

namespace Unchord
{
    public abstract class UnchordCanvas : MonoBehaviour
    {
        protected static GameManager s_gameManager;
        protected static UIManager s_uiManager;

        protected virtual void Awake()
        {
            if (s_gameManager == null)
                s_gameManager = GameManager.Instance;

            if (s_uiManager == null)
                s_uiManager = UIManager.Instance;
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