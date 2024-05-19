using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Management.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace Management.Infrastructure.AzureStorage;

public class BlobImageRepository : IImageRepository
{
    private readonly BlobContainerClient _containerClient;

    public BlobImageRepository(IOptions<AzureStorageOptions> options)
    {
        BlobServiceClient serviceClient = new BlobServiceClient(options.Value.ConnectionString);
        _containerClient = serviceClient.GetBlobContainerClient(options.Value.ImageBlobContainerName);
    }

    public async Task<string> Upload(Stream stream, string extension)
    {
        string fileName = await GenerateRandonFileName(extension);
        BlobClient blobClient = _containerClient.GetBlobClient(fileName);
        stream.Position = 0;
        await blobClient.UploadAsync(stream);
        return fileName;
    }

    public string? GetImageUrl(string fileName)
    {
        BlobClient blobClient = _containerClient.GetBlobClient(fileName);
        Uri? uri = CreateServiceSASBlob(blobClient);
        return uri?.ToString();
    }

    private async Task<string> GenerateRandonFileName(string extension)
    {
        while (true)
        {
            string fileName = Path.ChangeExtension(Path.GetRandomFileName(), extension);
            bool exist = await Exist(fileName);
            if (!exist)
            {
                return fileName;
            }
        }
    }

    private async Task<bool> Exist(string blobName)
    {
        BlobClient blobClient = _containerClient.GetBlobClient(blobName);
        Azure.Response<bool> response = await blobClient.ExistsAsync();
        return response.Value;
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
