using Management.Core.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Management.API.Controllers;

[Authorize]
[ApiController]
[Route("management/images")]
public class ImagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ImagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost(Name = "Upload")]
    public async Task<IActionResult> Upload(List<IFormFile> files)
    {
        List<string> fileNames = await _mediator.Send(new UploadImage.UploadImageRequest
        {
            Files = files,
        });

        return Ok(fileNames);
    }
}
