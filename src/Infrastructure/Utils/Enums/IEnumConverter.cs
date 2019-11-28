using System;

namespace Utils.Enums
{
    public interface IEnumConverter
    {
        string ToName<T>(object enumValue);
        T ToEnum<T>(string enumValue) where T : struct, Enum;
    }
}