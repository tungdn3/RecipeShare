﻿namespace Social.Core.Entities;

public class User
{
    public string Id { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string? Picture { get; set; }
}
