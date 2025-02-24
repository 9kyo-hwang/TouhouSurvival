using System;

namespace Unchord
{
    public abstract class AbilityAttributeSet<T_Enum> : AttributeSetBase<T_Enum>
    where T_Enum : System.Enum
    {
        public void RegisterEvent(T_Enum attrType,
            EventHandler<AttributeChangedEventArgs> eventHandler)
        {
            GameplayAttribute attribute = base.GetAttribute(attrType);

            UnityEngine.Debug.Assert(attribute != null);

            attribute.OnAttributeChanged += eventHandler;
        }

        public void UnregisterEvent(T_Enum attrType,
            EventHandler<AttributeChangedEventArgs> eventHandler)
        {
            GameplayAttribute attribute = base.GetAttribute(attrType);

            UnityEngine.Debug.Assert(attribute != null);

            attribute.OnAttributeChanged -= eventHandler;
        }
    }
}