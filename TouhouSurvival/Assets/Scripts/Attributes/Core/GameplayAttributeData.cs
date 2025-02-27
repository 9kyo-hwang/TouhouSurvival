using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unchord
{
    [Serializable]
    public struct GameplayAttributeData<T> where T : Enum
    {
        [Header("어트리뷰트 설정")] 
        public T attributeType;
        public float baseValue;
        public float minValue;
        public float maxValue;

        public GameplayAttributeData(T attributeType, float baseValue, float minValue, float maxValue)
        {
            this.attributeType = attributeType;
            this.baseValue = baseValue;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
    }
}