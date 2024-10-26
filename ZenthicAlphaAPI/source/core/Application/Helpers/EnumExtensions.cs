namespace Application.Helpers;

public static class EnumExtensions
{
    public static bool NotHasFlag(this Enum value, Enum flag)
        => !value.HasFlag(flag);

    public static TEnum AddFlag<TEnum>(this TEnum value, TEnum flag) where TEnum : struct, Enum
        => (dynamic)value | (dynamic)flag;

    public static TEnum RemoveFlag<TEnum>(this TEnum value, TEnum flag) where TEnum : struct, Enum
        => (dynamic)value & ~(dynamic)flag;

    public static string[] AsArray(this Enum value)
        => value.ToString().Split(", ");
}