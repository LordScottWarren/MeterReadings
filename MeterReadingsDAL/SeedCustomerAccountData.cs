using CsvHelper;
using MeterReadingsDAL.Helpers;
using MeterReadingsDAL.Models;
using System;
using System.Formats.Asn1;
using System.Globalization;

namespace MeterReadingsDAL;

public static class SeedCustomerAccountData
{
    public static void SeedTestData(this IMeterReadingDbContext context)
    {
        using var reader = ResourceReaderHelper.GetEmbeddedCsv("Test_Accounts - in.csv");
        string content = reader.ReadToEnd();

        List<CustomerAccount> customerAccounts = CsvParserHelper.ParseCustomerAccounts(content);

        context.CustomerAccounts.AddRange(customerAccounts);

        context.SaveChanges();
    }
}