namespace Search.API.Configurations;

public class AzureStorageOptions
{
    public string ConnectionString { get; set; } = string.Empty;

    public string ImageBlobContainerName { get; set; } = "images";
}
