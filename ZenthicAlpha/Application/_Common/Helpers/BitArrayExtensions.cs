using System.Collections;

namespace Application._Common.Helpers;

public static class BitArrayExtensions
{
    public static int ToInt(this BitArray value)
    {
        var bytes = new byte[1];
        value.CopyTo(bytes, 0);

        return bytes[0];
    }
}
