using UnityEngine;

namespace Unchord
{
    public abstract class ItemComponent : MonoBehaviour
    {
        public bool allowMagnetCollecting = true;

        public abstract void Use(Player player);

        protected virtual void OnEnable()
        {

        }
    }
}