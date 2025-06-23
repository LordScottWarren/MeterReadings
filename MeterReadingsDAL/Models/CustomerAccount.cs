using System.ComponentModel.DataAnnotations;

namespace MeterReadingsDAL.Models;

public class CustomerAccount
{
    [Key]
    public int AccountId { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public List<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();
}
