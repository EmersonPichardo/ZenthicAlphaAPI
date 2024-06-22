namespace Application._Common.Exceptions;

public class PasswordChangeRequiredException : Exception
{
    private const string baseMessage = "Password change required";

    public PasswordChangeRequiredException()
        : base(baseMessage) { }

    public PasswordChangeRequiredException(Exception innerException)
        : base(baseMessage, innerException) { }

}
