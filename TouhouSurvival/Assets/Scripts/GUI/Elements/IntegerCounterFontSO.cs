using UnityEngine;

namespace Unchord
{
    [CreateAssetMenu(fileName = "New Integer Counter Font Asset", menuName = "Scriptable Objects/UI/Integer Counter Font Asset", order = (int)GameManagerAssetMenuOrder.BossPhaseSO)]
    public class IntegerCounterFontSO : ScriptableObject
    {
        public Sprite[] digitSprites;
    }
}