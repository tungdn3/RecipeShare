// Ignore Spelling: validators

using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Management.Core.MediatorPipelines;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        ValidationResult[] validationFailures = await Task.WhenAll(_validators.Select(validator => validator.ValidateAsync(context)));

        List<ValidationFailure> errors = validationFailures
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .ToList();

        if (errors.Count > 0)
        {
            throw new Exceptions.ValidationException(errors);
        }

        var response = await next();

        return response;
    }
}
