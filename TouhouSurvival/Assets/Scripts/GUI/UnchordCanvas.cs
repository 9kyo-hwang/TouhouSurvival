using UnityEngine;

namespace Unchord
{
    public abstract class UnchordCanvas : MonoBehaviour
    {
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