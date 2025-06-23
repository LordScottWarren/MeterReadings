using System.ComponentModel.DataAnnotations;

namespace MeterReadingsDAL.Models;

public class MeterReading
{
    [Key]
    public Guid MeterReadingId { get; set; }

    public int CustomerAccountId { get; set; }
    public CustomerAccount CustomerAccount { get; set; }

    public DateTime DateTime { get; set; }
    public int MeterReadingValue { get; set; }
}
