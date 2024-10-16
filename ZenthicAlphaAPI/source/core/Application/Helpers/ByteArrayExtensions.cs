namespace Application.Helpers;

public static class ByteArrayExtensions
{
    public static byte[] Combine(this byte[] left, byte[] right)
    {
        var result = new byte[left.Length + right.Length];

        Buffer.BlockCopy(left, 0, result, 0, left.Length);
        Buffer.BlockCopy(right, 0, result, left.Length, right.Length);

        return result;
    }

    public static (byte[], byte[]) Split(this byte[] values, int length)
    {
        var bytes1 = new byte[length];
        var bytes2 = new byte[values.Length - length];

        Buffer.BlockCopy(values, 0, bytes1, 0, bytes1.Length);
        Buffer.BlockCopy(values, bytes1.Length, bytes2, 0, bytes2.Length);

        return (bytes1, bytes2);
    }
}
