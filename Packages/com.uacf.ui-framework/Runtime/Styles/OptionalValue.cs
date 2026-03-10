using System;
using UnityEngine;

namespace UACF.UI.Styles
{
    [Serializable]
    public struct OptionalValue<T>
    {
        public bool hasValue;
        public T value;

        public OptionalValue(T value)
        {
            hasValue = true;
            this.value = value;
        }

        public T GetValueOrDefault(T defaultValue)
        {
            return hasValue ? value : defaultValue;
        }
    }
}
