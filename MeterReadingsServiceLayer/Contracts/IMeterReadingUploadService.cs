using Shared.Models;

namespace MeterReadingsServiceLayer.Contracts;

public interface IMeterReadingUploadService
{
    Task<MeterReadingUploadResult> ProcessCsvAsync(string stream);
}
