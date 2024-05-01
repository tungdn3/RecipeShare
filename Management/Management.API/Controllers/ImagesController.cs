using Management.Core.Models;
using Management.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Management.API.Controllers;

[Authorize]
[ApiController]
[Route("management/images")]
public class ImagesController : ControllerBase
{
    private readonly IImageService _imageService;

    public ImagesController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpPost(Name = "Upload")]
    public async Task<IActionResult> Upload(List<IFormFile> files)
    {
        ImageUpload model = new() { Files = files };
        List<string> fileNames = await _imageService.Upload(model);

        return Ok(fileNames);
    }
}
