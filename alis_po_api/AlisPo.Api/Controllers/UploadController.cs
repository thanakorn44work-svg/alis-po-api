using Microsoft.AspNetCore.Mvc;

namespace AlisPo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UploadController : ControllerBase
{
    [HttpPost("image")]
    public async Task<ActionResult> UploadImage(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        var uploadsFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "uploads",
            "products");

        Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(file.FileName);

        var fileName =
            $"{Guid.NewGuid():N}{extension}";

        var fullPath = Path.Combine(
            uploadsFolder,
            fileName);

        await using var stream =
            System.IO.File.Create(fullPath);

        await file.CopyToAsync(
            stream,
            cancellationToken);

        return Ok(new
        {
            imagePath = $"uploads/products/{fileName}"
        });
    }
}