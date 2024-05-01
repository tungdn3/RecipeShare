using Microsoft.AspNetCore.Http;

namespace Management.Core.Models;

public class ImageUpload
{
    public List<IFormFile> Files { get; set; } = [];
}
