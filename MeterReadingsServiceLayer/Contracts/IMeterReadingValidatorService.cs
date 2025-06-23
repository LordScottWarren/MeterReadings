namespace MeterReadingsServiceLayer.Contracts;

public interface IMeterReadingValidatorService
{
    bool ValidateMeterReading(MeterReading meterReading);
}
