namespace Social.Infrastructure.Extensions;

public class InfrastructureOptions
{
    public string ServiceBusConnectionString { get; set; } = string.Empty;

    public string SqlLiteConnectionString { get; set; } = string.Empty;

    public string SqlConnectionString { get; set; } = string.Empty;
}
