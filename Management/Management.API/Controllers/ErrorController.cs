using FluentValidation;
using Management.API.Models;
using Management.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Management.API.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;
    private readonly IHostEnvironment _hostEnvironment;

    public ErrorController(ILogger<ErrorController> logger, IHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }

    [Route("/error")]
    public IActionResult HandleError()
    {
        var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;

        _logger.LogError(exceptionHandlerFeature.Error, "An error has occurred.");
        
        InnerError? innerError = _hostEnvironment.IsDevelopment()
            ? new InnerError { Trace = exceptionHandlerFeature.Error.StackTrace }
            : null;

        if (exceptionHandlerFeature.Error is ValidationException validationException)
        {
            var error = new Error
            {
                Code = ManagementConstants.ErrorCodes.MalformedValue,
                Message = "Validation failed",
                Details = validationException.Errors.Select(x => new Error
                {
                    Code = x.ErrorCode,
                    Message = x.ErrorMessage,
                    Target = x.PropertyName,
                }),
                InnerError = innerError
            };

            return BadRequest(error);
        }

        if (exceptionHandlerFeature.Error is BusinessLogicException businessLogicException)
        {
            return BadRequest(new Error
            {
                Code = businessLogicException.ErrorCode,
                Message = businessLogicException.Message,
                InnerError = innerError,
            });
        }

        var unknownError = new Error
        {
            Code = "InternalServerError",
            Message = _hostEnvironment.IsDevelopment()
                ? exceptionHandlerFeature.Error.Message
                : "An error happened while processing the request.",
            InnerError = innerError,
        };
        return StatusCode((int)HttpStatusCode.InternalServerError, unknownError);
    }
}
