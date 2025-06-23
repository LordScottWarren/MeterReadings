using MeterReadingsDAL.Models;

namespace MeterReadingsDAL.Repositories;

public interface IMeterReadingInsertRepository
{
    Task<bool> InsertAsync(MeterReading reading);
}

public class MeterReadingInsertRepository : IMeterReadingInsertRepository
{
    private readonly IMeterReadingDbContext dbConext;
    private readonly ICustomerAccountReadRepository customerAccountReadRepository;
    private readonly IMeterReadingReadRepository meterReadingReadRepository;

    public MeterReadingInsertRepository(IMeterReadingDbContext dbConext,
        ICustomerAccountReadRepository customerAccountReadRepository,
        IMeterReadingReadRepository meterReadingReadRepository)
    {
        this.dbConext = dbConext;
        this.customerAccountReadRepository = customerAccountReadRepository;
        this.meterReadingReadRepository = meterReadingReadRepository;
    }

    public async Task<bool> InsertAsync(MeterReading reading)
    {
        // Validates that there is a customer account
        CustomerAccount? customerAccount = await customerAccountReadRepository.GetByIdAsync(reading.CustomerAccountId);
        if (customerAccount == null)
        {
            return false;
        }

        // Validates there is not already a meter reading of the same value
        List<MeterReading> readings = await meterReadingReadRepository.GetByCustomerIdAsync(reading.CustomerAccountId);
        if (readings.Any(x => x.MeterReadingValue == reading.MeterReadingValue))
        {
            return false;
        }

        //Validates there is not an existing meter reading that is newere
        if(readings.Any() && readings.OrderByDescending(x => x.DateTime).First().DateTime >=  reading.DateTime)
        {
            return false;
        }

        dbConext.MeterReadings.Add(reading);
        await dbConext.SaveChangesAsync();

        return true;
    }
}
