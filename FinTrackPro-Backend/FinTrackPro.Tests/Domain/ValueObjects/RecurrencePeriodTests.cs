using FinTrackPro.Domain.Enums;
using FinTrackPro.Domain.ValueObjects;

namespace FinTrackPro.Tests.Domain.ValueObjects;

public class RecurrencePeriodTests
{
    [Theory]
    [InlineData(RecurrenceType.Daily, 1)]
    [InlineData(RecurrenceType.Weekly, 2)]
    [InlineData(RecurrenceType.Monthly, 3)]
    [InlineData(RecurrenceType.Yearly, 1)]
    public void Constructor_WithValidParameters_CreatesInstance(RecurrenceType type, int interval)
    {
        var period = new RecurrencePeriod(type, interval);

        Assert.Equal(type, period.RecurrenceType);
        Assert.Equal(interval, period.Interval);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithInvalidInterval_ThrowsArgumentException(int interval)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new RecurrencePeriod(RecurrenceType.Daily, interval));
        Assert.Contains("must be positive", exception.Message);
    }

    [Fact]
    public void Daily_CreatesCorrectPeriod()
    {
        var period = RecurrencePeriod.Daily(3);

        Assert.Equal(RecurrenceType.Daily, period.RecurrenceType);
        Assert.Equal(3, period.Interval);
    }

    [Fact]
    public void Weekly_CreatesCorrectPeriod()
    {
        var period = RecurrencePeriod.Weekly(2);

        Assert.Equal(RecurrenceType.Weekly, period.RecurrenceType);
        Assert.Equal(2, period.Interval);
    }

    [Fact]
    public void Biweekly_CreatesCorrectPeriod()
    {
        var period = RecurrencePeriod.Biweekly();

        Assert.Equal(RecurrenceType.Weekly, period.RecurrenceType);
        Assert.Equal(2, period.Interval);
    }

    [Fact]
    public void Monthly_CreatesCorrectPeriod()
    {
        var period = RecurrencePeriod.Monthly(6);

        Assert.Equal(RecurrenceType.Monthly, period.RecurrenceType);
        Assert.Equal(6, period.Interval);
    }

    [Fact]
    public void Quarterly_CreatesCorrectPeriod()
    {
        var period = RecurrencePeriod.Quarterly();

        Assert.Equal(RecurrenceType.Monthly, period.RecurrenceType);
        Assert.Equal(3, period.Interval);
    }

    [Fact]
    public void Yearly_CreatesCorrectPeriod()
    {
        var period = RecurrencePeriod.Yearly(2);

        Assert.Equal(RecurrenceType.Yearly, period.RecurrenceType);
        Assert.Equal(2, period.Interval);
    }

    [Fact]
    public void CalculateNextOccurrence_Daily_AddsCorrectDays()
    {
        var period = RecurrencePeriod.Daily(3);
        var startDate = new DateTime(2024, 1, 1);

        var nextDate = period.CalculateNextOccurrence(startDate);

        Assert.Equal(new DateTime(2024, 1, 4), nextDate);
    }

    [Fact]
    public void CalculateNextOccurrence_Weekly_AddsCorrectWeeks()
    {
        var period = RecurrencePeriod.Weekly(2);
        var startDate = new DateTime(2024, 1, 1);

        var nextDate = period.CalculateNextOccurrence(startDate);

        Assert.Equal(new DateTime(2024, 1, 15), nextDate);
    }

    [Fact]
    public void CalculateNextOccurrence_Monthly_AddsCorrectMonths()
    {
        var period = RecurrencePeriod.Monthly(3);
        var startDate = new DateTime(2024, 1, 15);

        var nextDate = period.CalculateNextOccurrence(startDate);

        Assert.Equal(new DateTime(2024, 4, 15), nextDate);
    }

    [Fact]
    public void CalculateNextOccurrence_Yearly_AddsCorrectYears()
    {
        var period = RecurrencePeriod.Yearly(2);
        var startDate = new DateTime(2024, 6, 15);

        var nextDate = period.CalculateNextOccurrence(startDate);

        Assert.Equal(new DateTime(2026, 6, 15), nextDate);
    }

    [Fact]
    public void Equals_SameTypeAndInterval_ReturnsTrue()
    {
        var period1 = RecurrencePeriod.Monthly(3);
        var period2 = RecurrencePeriod.Monthly(3);

        Assert.True(period1.Equals(period2));
        Assert.True(period1 == period2);
    }

    [Theory]
    [InlineData(RecurrenceType.Daily, 1, RecurrenceType.Daily, 2)]
    [InlineData(RecurrenceType.Daily, 1, RecurrenceType.Weekly, 1)]
    [InlineData(RecurrenceType.Monthly, 3, RecurrenceType.Yearly, 3)]
    public void Equals_DifferentValues_ReturnsFalse(RecurrenceType type1, int interval1, RecurrenceType type2, int interval2)
    {
        var period1 = new RecurrencePeriod(type1, interval1);
        var period2 = new RecurrencePeriod(type2, interval2);

        Assert.False(period1.Equals(period2));
        Assert.True(period1 != period2);
    }

    [Theory]
    [InlineData(RecurrenceType.Daily, 1, "Daily")]
    [InlineData(RecurrenceType.Weekly, 1, "Weekly")]
    [InlineData(RecurrenceType.Monthly, 1, "Monthly")]
    [InlineData(RecurrenceType.Daily, 3, "Every 3 daily")]
    [InlineData(RecurrenceType.Monthly, 6, "Every 6 monthly")]
    public void ToString_FormatsCorrectly(RecurrenceType type, int interval, string expected)
    {
        var period = new RecurrencePeriod(type, interval);

        Assert.Equal(expected, period.ToString());
    }

    [Fact]
    public void GetHashCode_SameValues_ReturnsSameHashCode()
    {
        var period1 = RecurrencePeriod.Monthly(3);
        var period2 = RecurrencePeriod.Monthly(3);

        Assert.Equal(period1.GetHashCode(), period2.GetHashCode());
    }
}
