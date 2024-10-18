namespace Infrastructure.Validations.ValidationErrorMessages;

public static class GenericValidationErrorMessage
{
    public const string Unauthorized = "Unauthorized";
    public const string Required = "Required";
    public const string InvalidFormat = "Invalid format";
    public const string OnlyNumbers = "Only numbers are allowed";
    public const string NotFound = "Not found";
    public const string Conflict = "Already registered";
    public const string RestrictedDeletion = "Unable to delete resource as it is in use by another resource";
    public const string MaximumLength = "Maximum length {MaxLength}";
    public const string MinimumLength = "Minimum length {MinLength}";
    public const string Length = "Length must be {MinLength}";
    public const string GreaterThan = "Must be greater than {ComparisonValue}";
    public const string LessThan = "Must be less than {ComparisonValue}";
    public const string Between = "Must be between {From} and {To}";
}
