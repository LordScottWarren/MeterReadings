using MeterReadingsServiceLayer.Contracts;

namespace MeterReadingsServiceLayer.Tests;

public class Tests
{
    // System under test: the validator service
    IMeterReadingValidatorService meterReadingValidator = new MeterReadingValidatorService();

    [SetUp]
    public void Setup()
    {
        // No setup needed for this service, but kept for structure/future use
    }

    /// <summary>
    /// Tests that invalid meter reading strings are correctly identified as invalid.
    /// </summary>
    /// <param name="meterReadingValue">The raw meter reading string to validate.</param>
    [TestCase("1002")]       // too short (less than 5 digits)
    [TestCase("999999")]     // too long (more than 5 digits)
    [TestCase("VOID")]       // non-numeric
    [TestCase("0X765")]      // invalid characters
    [TestCase("-6575")]      // negative number
    [TestCase("")]           // empty string
    public void IfMeterReadingHasIncorrectFormatReturnIsValidAsFalse(string meterReadingValue)
    {
        // Arrange
        var meterReading = new MeterReading()
        {
            MeterReadValue = meterReadingValue
        };

        // Act
        bool isValid = meterReadingValidator.ValidateMeterReading(meterReading);

        // Assert
        // These values are not considered valid meter readings (must be exactly 5 digits)
        Assert.That(isValid, Is.False,
            $"Expected meter reading '{meterReadingValue}' to be invalid, but it was marked valid.");
    }

    /// <summary>
    /// Tests that valid meter reading strings (exactly 5 numeric digits) are accepted.
    /// </summary>
    /// <param name="meterReadingValue">The raw meter reading string to validate.</param>
    [TestCase("45522")]
    [TestCase("23566")]
    public void IfMeterReadingHasCorrectFormatReturnIsValidAsTrue(string meterReadingValue)
    {
        // Arrange
        var meterReading = new MeterReading()
        {
            MeterReadValue = meterReadingValue
        };

        // Act
        bool isValid = meterReadingValidator.ValidateMeterReading(meterReading);

        // Assert
        // These are considered valid meter readings: exactly 5 digits, numeric
        Assert.That(isValid, Is.True,
            $"Expected meter reading '{meterReadingValue}' to be valid, but it was marked invalid.");
    }
}
