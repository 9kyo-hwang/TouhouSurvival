using UnityEngine;

namespace Unchord
{
    public abstract class Item : MonoBehaviour
    {
        public abstract void UseItem(Player player);
    }
}