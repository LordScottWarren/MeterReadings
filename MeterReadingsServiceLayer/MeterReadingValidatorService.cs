using MeterReadingsServiceLayer.Contracts;
using System.Text.RegularExpressions;

namespace MeterReadingsServiceLayer;

public class MeterReadingValidatorService : IMeterReadingValidatorService
{
    //Regex match on NNNNN format for meter reading value
    public bool ValidateMeterReading(MeterReading meterReading)
    {
        return Regex.IsMatch(meterReading.MeterReadValue, @"^\d{5}$");
    }
}
