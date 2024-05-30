using Social.Core.Dto;

namespace Social.Core.Exceptions;

public class ValidationException : Exception
{
    public IReadOnlyCollection<ValidationError> Errors { get; }

    public ValidationException()
    {
        Errors = new List<ValidationError>().AsReadOnly();
    }

    public ValidationException(IList<ValidationError> errors)
    {
        Errors = errors.AsReadOnly();
    }

    public ValidationException(string? message, IList<ValidationError> errors) : base(message)
    {
        Errors = errors.AsReadOnly();
    }

    public ValidationException(string? message, Exception? innerException, IList<ValidationError> errors) : base(message, innerException)
    {
        Errors = errors.AsReadOnly();
    }
}
