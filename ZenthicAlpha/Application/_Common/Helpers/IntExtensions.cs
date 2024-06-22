using Application._Common.Security.Authorization;
using System.Collections;

namespace Application._Common.Helpers;

public static class IntExtensions
{
    public static bool[] ToBoolArray(this int value)
        => new BitArray([value])
            .Cast<bool>()
            .Take(Enum.GetNames<Permission>().Length)
            .ToArray();
}
