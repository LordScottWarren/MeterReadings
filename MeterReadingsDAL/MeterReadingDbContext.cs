using MeterReadingsDAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MeterReadingsDAL;

public interface IMeterReadingDbContext
{
    DbSet<CustomerAccount> CustomerAccounts { get; set; }
    DbSet<MeterReading> MeterReadings { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
}

public class MeterReadingDbContext : DbContext, IMeterReadingDbContext
{
    public DbSet<CustomerAccount> CustomerAccounts { get; set; }
    public DbSet<MeterReading> MeterReadings { get; set; }

    public MeterReadingDbContext(DbContextOptions<MeterReadingDbContext> options)
            : base(options)
    {
    }

    public int SaveChanges()
    {
        return base.SaveChanges();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<MeterReading>()
            .HasOne(ca => ca.CustomerAccount)
            .WithMany(q => q.MeterReadings)
            .HasForeignKey(mr => mr.CustomerAccountId);
    }

}
