﻿using System.Runtime.Serialization;

namespace Management.Core.Exceptions;

[Serializable]
public class BusinessLogicException : Exception
{
    public BusinessLogicException()
    {
    }

    public BusinessLogicException(string? message) : base(message)
    {
    }

    public BusinessLogicException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected BusinessLogicException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
