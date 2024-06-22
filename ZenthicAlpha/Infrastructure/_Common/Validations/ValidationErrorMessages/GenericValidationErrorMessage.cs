namespace Infrastructure._Common.Validations.ValidationErrorMessages;

internal static class GenericValidationErrorMessage
{
    internal const string Required = "Required";
    internal const string InvalidFormat = "Invalid format";
    internal const string OnlyNumbers = "Only numbers are allowed";
    internal const string NotFound = "Not found";
    internal const string Conflict = "Already registered";
    internal const string RestrictedDeletion = "Unable to delete resource as it is in use by another resource";
    internal const string MaximumLength = "Maximum length {MaxLength}";
    internal const string MinimumLength = "Minimum length {MinLength}";
    internal const string Length = "Length must be {MinLength}";
    internal const string GreaterThan = "Must be greater than {ComparisonValue}";
    internal const string LessThan = "Must be less than {ComparisonValue}";
    internal const string Between = "Must be between {From} and {To}";
}
