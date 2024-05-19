using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using Search.API.Configurations;

namespace Search.API.Repositories;

public class BlobImageRepository
{
    private readonly BlobContainerClient _containerClient;

    public BlobImageRepository(IOptions<AzureStorageOptions> options)
    {
        BlobServiceClient serviceClient = new BlobServiceClient(options.Value.ConnectionString);
        _containerClient = serviceClient.GetBlobContainerClient(options.Value.ImageBlobContainerName);
    }

    public string? GetImageUrl(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return null;
        }

        BlobClient blobClient = _containerClient.GetBlobClient(fileName);
        Uri? uri = CreateServiceSASBlob(blobClient);
        return uri?.ToString();
    }

    // Copied from https://learn.microsoft.com/en-us/azure/storage/blobs/sas-service-create-dotnet#create-a-service-sas-for-a-blob
    private static Uri? CreateServiceSASBlob(
        BlobClient blobClient,
        string? storedPolicyName = null)
    {
        // Check if BlobContainerClient object has been authorized with Shared Key
        if (blobClient.CanGenerateSasUri)
        {
            // Create a SAS token that's valid for one day
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                BlobName = blobClient.Name,
                Resource = "b"
            };

            if (storedPolicyName == null)
            {
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddDays(1);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
            }
            else
            {
                sasBuilder.Identifier = storedPolicyName;
            }

            Uri sasURI = blobClient.GenerateSasUri(sasBuilder);

            return sasURI;
        }
        else
        {
            // Client object is not authorized via Shared Key
            return null;
        }
    }
}
