using CsvHelper.Configuration;
using System.Globalization;

public class MeterReadingMap : ClassMap<MeterReading>
{
    public MeterReadingMap()
    {
        Map(m => m.AccountId).Name("AccountId");

        Map(m => m.MeterReadingDateTime)
            .Name("MeterReadingDateTime")
            .TypeConverterOption
            .DateTimeStyles(DateTimeStyles.AssumeLocal)
            .TypeConverterOption
            .Format("dd/MM/yyyy HH:mm");

        Map(m => m.MeterReadValue).Name("MeterReadValue");
    }
}