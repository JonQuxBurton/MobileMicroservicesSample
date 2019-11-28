using System;

namespace Utils.Enums
{
    public class EnumConverter : IEnumConverter
    {
        public string ToName<T>(object enumValue)
        {
            return Enum.GetName(typeof(T), enumValue);
        }

        public T ToEnum<T>(string enumValue) where T : struct, Enum
        {
            Enum.TryParse(enumValue.Trim(), out T state);
            return state;
        }
    }
}
