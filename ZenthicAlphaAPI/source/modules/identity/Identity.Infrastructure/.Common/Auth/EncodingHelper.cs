using System.Text;

namespace Identity.Infrastructure.Common.Auth;

internal static class EncodingHelper
{
    private const string encodingName = "UTF-8";

    public static byte[] GetBytes(string text) => Encoding.GetEncoding(encodingName).GetBytes(text);
    public static string GetString(byte[] data) => Encoding.GetEncoding(encodingName).GetString(data);
}
