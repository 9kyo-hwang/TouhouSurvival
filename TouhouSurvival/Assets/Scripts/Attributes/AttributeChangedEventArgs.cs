using System;

namespace Unchord
{
    public class AttributeChangedEventArgs : EventArgs
    {
        public float OldValue { get; private set; }
        public float NewValue { get; private set; }

        public AttributeChangedEventArgs(float oldValue, float newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}