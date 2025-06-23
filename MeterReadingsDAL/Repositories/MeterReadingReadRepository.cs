using MeterReadingsDAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MeterReadingsDAL.Repositories;

public interface IMeterReadingReadRepository
{
    Task<List<MeterReading>> GetByCustomerIdAsync(int accountId);
}

public class MeterReadingReadRepository : IMeterReadingReadRepository
{
    private readonly IMeterReadingDbContext dbConext;

    public MeterReadingReadRepository(IMeterReadingDbContext dbConext)
    {
        this.dbConext = dbConext;
    }

    public async Task<List<MeterReading>> GetByCustomerIdAsync(int accountId)
    {
        return await this.dbConext
            .MeterReadings
            .AsNoTracking()
            .Where(x => x.CustomerAccountId == accountId)
            .ToListAsync();
    }
}
