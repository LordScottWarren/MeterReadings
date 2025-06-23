namespace Shared.Models;

public class MeterReadingUploadResult
{
    public bool Success { get; set; }
    public int SuccessfulUploads { get; set; }
    public int FailedUploads { get; set; }
}

