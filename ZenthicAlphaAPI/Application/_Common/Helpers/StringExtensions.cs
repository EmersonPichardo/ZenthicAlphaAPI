using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Application._Common.Helpers;

public static class StringExtensions
{
    public static string ToCamelCase(this string value)
        => JsonNamingPolicy.CamelCase.ConvertName(value.Replace(" ", string.Empty));

    public static string ToNormalize(this string value)
    {
        var charsWithoutAccents = value
            .Normalize(NormalizationForm.FormD)
            .Where(character
                => CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark
            )
            .ToArray();

        var normalizedValue = string
            .Concat(charsWithoutAccents)
            .ToLower()
            .Trim();

        return normalizedValue;
    }
}