// Ignore Spelling: Validator

using FluentValidation;
using Management.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Management.Core.Commands;

public static class UploadImage
{
    public class UploadImageRequest : IRequest<List<string>>
    {
        public List<IFormFile> Files { get; set; } = [];
    }

    public class Handler : IRequestHandler<UploadImageRequest, List<string>>
    {
        private readonly IImageRepository _imageRepository;

        public Handler(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public async Task<List<string>> Handle(UploadImageRequest request, CancellationToken cancellationToken)
        {
            List<string> savedFileNames = [];
            foreach (IFormFile formFile in request.Files)
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

    public class Validator : AbstractValidator<UploadImageRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Files)
                .NotEmpty();

            RuleForEach(x => x.Files).ChildRules(f =>
            {
                f.RuleFor(x => x.Length).LessThanOrEqualTo(5_242_880); // 5MB
                                                                       // todo: check extensions, scan virus???
            });
        }
    }
}
