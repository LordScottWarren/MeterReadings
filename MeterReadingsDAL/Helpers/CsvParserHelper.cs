using CsvHelper;
using MeterReadingsDAL.Mappers;
using MeterReadingsDAL.Models;
using System.Globalization;

namespace MeterReadingsDAL.Helpers;

public class CsvParserHelper
{
    /// <summary>
    /// parses the content of a csv string into a list of customer accounts
    /// </summary>
    /// <param name="csvContent"></param>
    /// <returns></returns>
    public static List<CustomerAccount> ParseCustomerAccounts(string csvContent)
    {
        using var reader = new StringReader(csvContent);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Context.RegisterClassMap<CustomerAccountMap>();

        var records = new List<CustomerAccount>();
        foreach (var record in csv.GetRecords<CustomerAccount>())
        {
            records.Add(record);
        }

        return records;
    }
}
