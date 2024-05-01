using FluentValidation;
using Management.Core.Interfaces;
using Management.Core.Models;
using Management.Core.Validators;

namespace Management.Core.Services;

public class ImageService : IImageService
{
    private readonly IImageRepository _imageRepository;
    private readonly IValidator<ImageUpload> _imageUploadValidator;

    public ImageService(IImageRepository imageRepository, IValidator<ImageUpload> imageUploadValidator)
    {
        _imageRepository = imageRepository;
        _imageUploadValidator = imageUploadValidator;
    }

    public async Task<List<string>> Upload(ImageUpload model)
    {
        _imageUploadValidator.ValidateAndThrow(model);

        List<string> savedFileNames = [];
        foreach (var formFile in model.Files)
        {
            if (formFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await formFile.CopyToAsync(memoryStream);
                string ext = Path.GetExtension(formFile.FileName).ToLowerInvariant();
                string fileName = await _imageRepository.Upload(memoryStream, ext);
                savedFileNames.Add(fileName);
            }
        }

        return savedFileNames;
    }
}
