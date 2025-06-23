using MeterReadingsDAL.Repositories;
using MeterReadingsServiceLayer.Contracts;
using Moq;
using Shared.Models;
using System.Text;

namespace MeterReadingsServiceLayer.Tests;

public class MeterReadingUploadServiceTests
{
    // System under test: the meter reading upload service
    IMeterReadingUploadService meterReadingUploadService;
    Mock<IMeterReadingInsertRepository> insertRepoMock = new Mock<IMeterReadingInsertRepository>();
    Mock<IMeterReadingValidatorService> validatorMock = new Mock<IMeterReadingValidatorService>();

    [SetUp]
    public void Setup()
    {
        validatorMock.Setup(v => v.ValidateMeterReading(It.IsAny<MeterReading>()))
                     .Returns<MeterReading>(r => r.MeterReadValue != "VOID");

        meterReadingUploadService = new MeterReadingUploadService(validatorMock.Object, insertRepoMock.Object);
    }

    /// <summary>
    /// Tests that invalid meter reading strings are correctly identified as invalid.
    /// </summary>
    /// 
    [Test]
    public async Task ProcessCsvAsync_InsertsValidReadings()
    {
        StringBuilder csvContent = new StringBuilder();
        csvContent.AppendLine("AccountId,MeterReadingDateTime,MeterReadValue");
        csvContent.AppendLine("2344,22/04/2019 09:24,1002");
        csvContent.AppendLine("2233,22/04/2019 12:25,VOID");

        await meterReadingUploadService.ProcessCsvAsync(csvContent.ToString());

        insertRepoMock.Verify(x => x.InsertAsync(It.Is<MeterReadingsDAL.Models.MeterReading>(
            mr => mr.CustomerAccountId == 2344 && mr.MeterReadingValue == 1002
        )), Times.Once);

        insertRepoMock.Verify(x => x.InsertAsync(It.IsAny<MeterReadingsDAL.Models.MeterReading>()), Times.Once);
    }

    /// <summary>
    /// Tests that invalid meter reading strings are correctly identified as invalid.
    /// </summary>
    /// 
    [Test]
    public async Task ProcessCsvAsync_ReturnsAccurateResponse()
    {
        StringBuilder csvContent = new StringBuilder();
        csvContent.AppendLine("AccountId,MeterReadingDateTime,MeterReadValue");
        csvContent.AppendLine("2344,22/04/2019 09:24,1002");
        csvContent.AppendLine("2233,22/04/2019 12:25,VOID");
        csvContent.AppendLine("2239,22/04/2019 12:25,VOID");

        insertRepoMock.Setup(x => x.InsertAsync(It.IsAny<MeterReadingsDAL.Models.MeterReading>())).ReturnsAsync(true);

        MeterReadingUploadResult result = await meterReadingUploadService.ProcessCsvAsync(csvContent.ToString());

        Assert.That(result, Is.Not.Null);
        Assert.That(result.SuccessfulUploads == 1);
        Assert.That(result.FailedUploads == 2);
    }
}
