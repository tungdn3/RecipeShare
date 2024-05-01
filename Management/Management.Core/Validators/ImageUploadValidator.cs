using FluentValidation;
using Management.Core.Models;

namespace Management.Core.Validators;

public class ImageUploadValidator : AbstractValidator<ImageUpload>
{
    public ImageUploadValidator()
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
