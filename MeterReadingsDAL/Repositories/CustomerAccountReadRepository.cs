using MeterReadingsDAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MeterReadingsDAL.Repositories;

public interface ICustomerAccountReadRepository
{
    Task<CustomerAccount?> GetByIdAsync(int accountId);
}

public class CustomerAccountReadRepository : ICustomerAccountReadRepository
{
    private readonly IMeterReadingDbContext dbConext;

    public CustomerAccountReadRepository(IMeterReadingDbContext dbConext)
    {
        this.dbConext = dbConext;
    }

    public async Task<CustomerAccount?> GetByIdAsync(int accountId)
    {
        return await this.dbConext
            .CustomerAccounts
            .AsNoTracking()
            .Where(x => x.AccountId == accountId)
            .FirstOrDefaultAsync();
    }
}
