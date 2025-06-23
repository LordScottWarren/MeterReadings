using CsvHelper;
using System.Globalization;

namespace MeterReadingsServiceLayer.Helpers;

public static class CsvParserHelper
{
    public static async Task<List<MeterReading>> ParseCSV(string csvContent)
    {
        using var reader = new StringReader(csvContent);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Context.RegisterClassMap<MeterReadingMap>();

        var records = new List<MeterReading>();
        await foreach (var record in csv.GetRecordsAsync<MeterReading>())
        {
            records.Add(record);
        }

        return records;
    }
}
