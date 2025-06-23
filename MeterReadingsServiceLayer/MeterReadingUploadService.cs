using MeterReadingsDAL.Repositories;
using MeterReadingsServiceLayer.Contracts;
using MeterReadingsServiceLayer.Helpers;
using Shared.Models;

namespace MeterReadingsServiceLayer;
public class MeterReadingUploadService : IMeterReadingUploadService
{
    private readonly IMeterReadingValidatorService validatorService;
    private readonly IMeterReadingInsertRepository meterReadingInsertRepository;

    public MeterReadingUploadService(IMeterReadingValidatorService validatorService, IMeterReadingInsertRepository meterReadingInsertRepository)
    {
        this.validatorService = validatorService;
        this.meterReadingInsertRepository = meterReadingInsertRepository;
    }

    public async Task<MeterReadingUploadResult> ProcessCsvAsync(string csvContent)
    {
        MeterReadingUploadResult results = new MeterReadingUploadResult() { Success = true };

        var records = await CsvParserHelper.ParseCSV(csvContent);

        foreach (var meterReading in records)
        {
            bool isValid = validatorService.ValidateMeterReading(meterReading);
            if (isValid)
            {
                var newMeterReading = new MeterReadingsDAL.Models.MeterReading
                {
                    CustomerAccountId = meterReading.AccountId,
                    DateTime = meterReading.MeterReadingDateTime,
                    MeterReadingValue = int.Parse(meterReading.MeterReadValue),
                };

                if(await meterReadingInsertRepository.InsertAsync(newMeterReading))
                {
                    ++results.SuccessfulUploads;
                }
                else
                {
                    ++results.FailedUploads;
                }
            }
            else
            {
                ++results.FailedUploads;
            }
        }

        return results;
    }

    
}
