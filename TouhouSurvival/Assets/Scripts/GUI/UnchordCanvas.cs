using UnityEngine;

namespace Unchord
{
    public abstract class UnchordCanvas : MonoBehaviour
    {
        protected static GameManager s_gameManager;
        protected static UIManager s_uiManager;

        static UnchordCanvas()
        {
            s_gameManager = GameManager.Instance;
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