using CsvHelper.Configuration;
using MeterReadingsDAL.Models;

namespace MeterReadingsDAL.Mappers;

public class CustomerAccountMap : ClassMap<CustomerAccount>
{
    public CustomerAccountMap()
    {
        Map(m => m.AccountId).Name("AccountId");
        Map(m => m.FirstName).Name("FirstName");
        Map(m => m.LastName).Name("LastName");
    }
}