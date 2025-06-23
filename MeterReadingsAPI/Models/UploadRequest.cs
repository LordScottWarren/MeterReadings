using Microsoft.AspNetCore.Mvc;

namespace MeterReadingsAPI.Models;

public class UploadRequest
{
    [FromForm(Name = "file")]
    public IFormFile File { get; set; }
}
