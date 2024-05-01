using Management.Core.Models;

namespace Management.Core.Services
{
    public interface IImageService
    {
        Task<List<string>> Upload(ImageUpload model);
    }
}