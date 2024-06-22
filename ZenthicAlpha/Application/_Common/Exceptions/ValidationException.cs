using FluentValidation.Results;
using System.Dynamic;

namespace Application._Common.Exceptions;

public class ValidationException : Exception
{
    public IDictionary<string, dynamic?> Errors { get; }

    private const string propertyNameIdentifier = "PropertyName";
    private const string arrayIdentifier = "CollectionIndex";

    public ValidationException(IEnumerable<ValidationFailure> failures)
    {
        var notArrayErrors = failures
            .Where(IsNotArray)
            .GroupBy(
                GetPropertyName,
                failure => failure.ErrorMessage
            );

        var arrayErrors = failures
            .Where(IsArray)
            .GroupBy(
                GetPropertyName,
                ValidationFailureSimple.FromValidationFailure
            );

        var errors = new Dictionary<string, dynamic?>();

        foreach (var keyValuePair in notArrayErrors)
            errors.Add(keyValuePair.Key, keyValuePair.First());

        foreach (var keyValuePair in arrayErrors)
            errors.Add(keyValuePair.Key, GetValidationFailureObjectArray(keyValuePair));

        Errors = errors;
    }

    private static IEnumerable<dynamic?> GetValidationFailureObjectArray(IGrouping<string, ValidationFailureSimple> failureGroup)
    {
        static dynamic dynamicMapping(IEnumerable<ValidationFailureSimple> failures)
        {
            dynamic result = new ExpandoObject();
            IDictionary<string, object> properties = result;

            foreach (var failure in failures)
            {
                properties.Add(
                    failure
                        .Values[propertyNameIdentifier]
                        .ToString()?
                        .ToCamelCase()
                    ?? string.Empty,

                    failure.ErrorMessage
                );
            }

            return result;
        }

        return failureGroup
            .GroupBy(
                failure => failure
                    .Values[arrayIdentifier]
                    .ToString() ?? string.Empty,
                (_, b) => dynamicMapping(b)
            );
    }
    private static bool IsArray(ValidationFailure failure)
        => failure.FormattedMessagePlaceholderValues?.ContainsKey(arrayIdentifier) ?? false;
    private static bool IsNotArray(ValidationFailure failure)
        => !IsArray(failure);
    private static string GetPropertyName(ValidationFailure failure)
        => IsArray(failure)
            ? failure.PropertyName.Split('[')[0].ToCamelCase()
            : failure.PropertyName.ToCamelCase();
    private sealed record ValidationFailureSimple
    {
        public required string ErrorMessage { get; init; }
        public required Dictionary<string, object> Values { get; init; }

        public static ValidationFailureSimple FromValidationFailure(ValidationFailure failure)
            => new()
            {
                ErrorMessage = failure.ErrorMessage,
                Values = failure.FormattedMessagePlaceholderValues
            };
    }
}