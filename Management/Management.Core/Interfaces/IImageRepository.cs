namespace Management.Core.Interfaces
{
    public interface IImageRepository
    {
        string? GetImageUrl(string fileName);

        Task<string> Upload(Stream stream, string extension);
    }
}