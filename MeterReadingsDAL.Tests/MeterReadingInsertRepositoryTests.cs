using MeterReadingsDAL.Models;
using MeterReadingsDAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace MeterReadingsDAL.Tests;

public class Tests
{
    // System under test
    MeterReadingInsertRepository meterReadingInsertRepository;

    // Mock dependencies
    Mock<ICustomerAccountReadRepository> customerAccountReadRepository = new Mock<ICustomerAccountReadRepository>();
    Mock<IMeterReadingReadRepository> meterReadingReadRepository = new Mock<IMeterReadingReadRepository>();
    Mock<IMeterReadingDbContext> dbContext = new Mock<IMeterReadingDbContext>();

    // Called before each test runs
    [SetUp]
    public void Setup()
    {
        // Initialize the SUT with mocked dependencies
        customerAccountReadRepository = new Mock<ICustomerAccountReadRepository>();
        meterReadingReadRepository = new Mock<IMeterReadingReadRepository>();
        dbContext = new Mock<IMeterReadingDbContext>();
        meterReadingInsertRepository = new MeterReadingInsertRepository(dbContext.Object,
            customerAccountReadRepository.Object,
            meterReadingReadRepository.Object);
    }

    /// <summary>
    /// Creates a mock DbSet backed by an in-memory list.
    /// Simulates basic EF Core behavior for LINQ and .Add().
    /// </summary>
    private Mock<DbSet<MeterReading>> CreateMockDbSet(List<MeterReading> sourceList)
    {
        var queryable = sourceList.AsQueryable();
        var mockSet = new Mock<DbSet<MeterReading>>();

        // Setup IQueryable behavior for LINQ queries
        mockSet.As<IQueryable<MeterReading>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<MeterReading>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<MeterReading>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<MeterReading>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

        // Hook into .Add() to simulate EF insert behavior
        mockSet.Setup(d => d.Add(It.IsAny<MeterReading>()))
               .Callback<MeterReading>(r => sourceList.Add(r));

        return mockSet;
    }

    /// <summary>
    /// Verifies that a meter reading is inserted if the corresponding customer account exists.
    /// </summary>
    [Test]
    public async Task InsertMeterReadingOnlyWhenThereIsAValidCustomerAccount()
    {
        // Arrange
        var readings = new List<MeterReading>();
        var mockMeterReadingSet = CreateMockDbSet(readings);
        dbContext.Setup(x => x.MeterReadings).Returns(mockMeterReadingSet.Object);
        meterReadingReadRepository.Setup(x => x.GetByCustomerIdAsync(1)).ReturnsAsync(readings);

        Guid meterReadingId = Guid.NewGuid();
        MeterReading meterReading = new MeterReading()
        {
            MeterReadingId = meterReadingId,
            CustomerAccountId = 1,
            DateTime = DateTime.Now,
            MeterReadingValue = 1234
        };

        // Simulate existing customer account with ID 1
        customerAccountReadRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new CustomerAccount() { AccountId = 1 });

        // Act
        await meterReadingInsertRepository.InsertAsync(meterReading);

        // Assert
        // The reading should be added because the customer exists
        Assert.That(readings.Any(x => x.MeterReadingId == meterReadingId), Is.True);
    }

    /// <summary>
    /// Verifies that a meter reading is NOT inserted if the customer account doesn't exist.
    /// </summary>
    [Test]
    public async Task DoNotInsertMeterReadingWhenThereIsNoValidCustomerAccount()
    {
        // Arrange
        var readings = new List<MeterReading>();
        var mockMeterReadingSet = CreateMockDbSet(readings);
        dbContext.Setup(x => x.MeterReadings).Returns(mockMeterReadingSet.Object);
        meterReadingReadRepository.Setup(x => x.GetByCustomerIdAsync(2)).ReturnsAsync(readings);

        Guid meterReadingId = Guid.NewGuid();
        MeterReading meterReading = new MeterReading()
        {
            MeterReadingId = meterReadingId,
            CustomerAccountId = 2, // This account will NOT be found
            DateTime = DateTime.Now,
            MeterReadingValue = 1234
        };

        // Setup returns null by NOT matching the provided ID
        customerAccountReadRepository
            .Setup(x => x.GetByIdAsync(1)) // only sets up account with ID 1
            .ReturnsAsync(new CustomerAccount() { AccountId = 1 });

        // Act
        await meterReadingInsertRepository.InsertAsync(meterReading);

        // Assert
        // Since account ID 2 is not valid in the test setup, no insert should happen
        Assert.That(readings.Any(x => x.MeterReadingId == meterReadingId), Is.False);
    }

    /// <summary>
    /// Verifies that a meter reading can not be added more than once to a customers account
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task TheSameMeterReadingCanNotBeEnteredTwice()
    {
        // Arrange
        var readings = new List<MeterReading>();
        var mockMeterReadingSet = CreateMockDbSet(readings);
        dbContext.Setup(x => x.MeterReadings).Returns(mockMeterReadingSet.Object);

        meterReadingReadRepository.Setup(x => x.GetByCustomerIdAsync(2)).ReturnsAsync(readings);

        MeterReading meterReading = new MeterReading()
        {
            MeterReadingId = Guid.NewGuid(),
            CustomerAccountId = 2,
            DateTime = DateTime.Now,
            MeterReadingValue = 1234
        };
        MeterReading meterReadingDuplicate = new MeterReading()
        {
            MeterReadingId = Guid.NewGuid(),
            CustomerAccountId = 2,
            DateTime = DateTime.Now,
            MeterReadingValue = 1234
        };

        // Simulate existing customer account with ID 2
        customerAccountReadRepository
            .Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(new CustomerAccount() { AccountId = 2 });

        // Act
        await meterReadingInsertRepository.InsertAsync(meterReading);
        await meterReadingInsertRepository.InsertAsync(meterReadingDuplicate);

        // Assert
        // Since account ID 2 is not valid in the test setup, no insert should happen
        Assert.That(readings.Count(x => x.CustomerAccountId == 2) == 1, Is.True);
    }

    /// <summary>
    /// Verifies that a meter reading is NOT inserted if the customer account has previous readings that are newer than the inserting record
    /// </summary>
    /// 
    [Test]
    public async Task DoesNotInsertMeterReading_WhenOlderThanExistingReadings()
    {
        // Arrange
        var readings = new List<MeterReading>
        {
            new MeterReading()
            {
                MeterReadingId = Guid.NewGuid(),
                CustomerAccountId = 2,
                DateTime = new DateTime(2025,6,3),
                MeterReadingValue = 1234
            }
        };

        var mockMeterReadingSet = CreateMockDbSet(readings);
        dbContext.Setup(x => x.MeterReadings).Returns(mockMeterReadingSet.Object);

        meterReadingReadRepository.Setup(x => x.GetByCustomerIdAsync(2)).ReturnsAsync(readings);

        Guid readingId = Guid.NewGuid();
        MeterReading meterReading = new MeterReading()
        {
            MeterReadingId = readingId,
            CustomerAccountId = 2,
            DateTime = new DateTime(2025,6,2),
            MeterReadingValue = 1222
        };

        // Simulate existing customer account with ID 2
        customerAccountReadRepository
            .Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(new CustomerAccount() { AccountId = 2 });

        // Act
        await meterReadingInsertRepository.InsertAsync(meterReading);

        // Assert
        // Since the new meter reading is older than the newest record for the account, no insert should happen
        Assert.That(readings.Count(x => x.MeterReadingId == readingId) == 0, Is.True);
    }

    /// <summary>
    /// Verifies that a meter reading IS inserted if the customer account has previous readings that are older than the inserting record
    /// </summary>
    /// 
    [Test]
    public async Task InsertsMeterReading_WhenNewerThanExistingReadings()
    {
        // Arrange
        var readings = new List<MeterReading>
        {
            new MeterReading()
            {
                MeterReadingId = Guid.NewGuid(),
                CustomerAccountId = 2,
                DateTime = new DateTime(2025,6,3),
                MeterReadingValue = 1234
            }
        };

        var mockMeterReadingSet = CreateMockDbSet(readings);
        dbContext.Setup(x => x.MeterReadings).Returns(mockMeterReadingSet.Object);

        meterReadingReadRepository.Setup(x => x.GetByCustomerIdAsync(2)).ReturnsAsync(readings);

        Guid readingId = Guid.NewGuid();
        MeterReading meterReading = new MeterReading()
        {
            MeterReadingId = readingId,
            CustomerAccountId = 2,
            DateTime = new DateTime(2025, 6, 5),
            MeterReadingValue = 1225
        };

        // Simulate existing customer account with ID 2
        customerAccountReadRepository
            .Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(new CustomerAccount() { AccountId = 2 });

        // Act
        await meterReadingInsertRepository.InsertAsync(meterReading);

        // Assert
        // Since the new meter reading is newer than the newest account record, insert should happen
        Assert.That(readings.Count(x => x.MeterReadingId == readingId) == 1, Is.True);
    }
}
