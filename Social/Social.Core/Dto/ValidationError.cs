﻿namespace Social.Core.Dto;

public class ValidationError
{
    public string Property { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
