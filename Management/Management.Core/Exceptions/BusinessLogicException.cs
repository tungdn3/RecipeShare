namespace Management.Core.Exceptions;

public class BusinessLogicException : Exception
{
    public string ErrorCode { get; private set; }

    public BusinessLogicException(string errorCode)
    {
        ErrorCode = errorCode;
    }

    public BusinessLogicException(string errorCode, string? message) : base(message)
    {
        ErrorCode = errorCode;
    }

    public BusinessLogicException(string errorCode, string? message, Exception? innerException) : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
