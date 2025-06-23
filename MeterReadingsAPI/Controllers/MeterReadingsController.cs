using MeterReadingsAPI.Models;
using MeterReadingsServiceLayer.Contracts;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace MeterReadings.Controllers;

[ApiController]
[Route("meter-reading-upload")]
public class MeterReadingsController : ControllerBase
{
    private readonly IMeterReadingUploadService uploadSerivce;

    public MeterReadingsController(IMeterReadingUploadService uploadSerivce)
    {
        this.uploadSerivce = uploadSerivce;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Post([FromForm] UploadRequest request)
    {
        var file = request.File;

        if (file == null || file.Length == 0)
        {
            return BadRequest("File is empty or missing.");
        }

        string csvContent;
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            csvContent = await reader.ReadToEndAsync();
        }

        MeterReadingUploadResult result = await uploadSerivce.ProcessCsvAsync(csvContent);

        return new OkObjectResult(result);
    }
}
