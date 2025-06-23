using System;

public class MeterReading
{
    public int AccountId { get; set; }

    public DateTime MeterReadingDateTime { get; set; }

    public string MeterReadValue { get; set; } = string.Empty;
}
