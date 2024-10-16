using Domain.Modularity;
using System.Collections;

namespace Application.Helpers;

public static class IntExtensions
{
    public static bool[] ToBoolArray(this int value)
        => new BitArray([value])
            .Cast<bool>()
            .Take(Enum.GetNames<Permission>().Length)
            .ToArray();
}
