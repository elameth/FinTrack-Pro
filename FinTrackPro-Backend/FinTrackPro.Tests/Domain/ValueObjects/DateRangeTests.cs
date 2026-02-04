using FinTrackPro.Domain.ValueObjects;

namespace FinTrackPro.Tests.Domain.ValueObjects;

public class DateRangeTests
{
    [Fact]
    public void Constructor_WithValidDates_CreatesInstance()
    {
        var start = new DateTime(2024, 1, 1);
        var end = new DateTime(2024, 1, 31);

        var range = new DateRange(start, end);

        Assert.Equal(start, range.StartDate);
        Assert.Equal(end, range.EndDate);
    }

    [Fact]
    public void Constructor_WithSameDate_CreatesInstance()
    {
        var date = new DateTime(2024, 1, 15);

        var range = new DateRange(date, date);

        Assert.Equal(date, range.StartDate);
        Assert.Equal(date, range.EndDate);
    }

    [Fact]
    public void Constructor_WithStartAfterEnd_ThrowsArgumentException()
    {
        var start = new DateTime(2024, 1, 31);
        var end = new DateTime(2024, 1, 1);

        var exception = Assert.Throws<ArgumentException>(() => new DateRange(start, end));
        Assert.Contains("cannot be after", exception.Message);
    }

    [Fact]
    public void Constructor_StripsTimeComponent()
    {
        var start = new DateTime(2024, 1, 1, 10, 30, 45);
        var end = new DateTime(2024, 1, 31, 23, 59, 59);

        var range = new DateRange(start, end);

        Assert.Equal(new DateTime(2024, 1, 1), range.StartDate);
        Assert.Equal(new DateTime(2024, 1, 31), range.EndDate);
    }

    [Theory]
    [InlineData(1, 1, 1, 1, 0)]
    [InlineData(1, 1, 1, 31, 30)]
    [InlineData(1, 1, 12, 31, 365)]
    public void Duration_CalculatesCorrectly(int startMonth, int startDay, int endMonth, int endDay, int expectedDays)
    {
        var range = new DateRange(
            new DateTime(2024, startMonth, startDay),
            new DateTime(2024, endMonth, endDay));

        Assert.Equal(expectedDays, range.Duration.TotalDays);
    }

    [Theory]
    [InlineData(1, 1, 1, 1, 1)]
    [InlineData(1, 1, 1, 31, 31)]
    [InlineData(1, 1, 12, 31, 366)]
    public void DurationInDays_IncludesStartAndEndDays(int startMonth, int startDay, int endMonth, int endDay, int expected)
    {
        var range = new DateRange(
            new DateTime(2024, startMonth, startDay),
            new DateTime(2024, endMonth, endDay));

        Assert.Equal(expected, range.DurationInDays);
    }

    [Theory]
    [InlineData(1, 15, true)]
    [InlineData(1, 1, true)]
    [InlineData(1, 31, true)]
    [InlineData(12, 31, false)]
    [InlineData(2, 1, false)]
    public void Contains_ChecksDateCorrectly(int month, int day, bool expected)
    {
        var range = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));
        var date = new DateTime(2024, month, day);

        Assert.Equal(expected, range.Contains(date));
    }

    [Fact]
    public void Overlaps_WithOverlappingRange_ReturnsTrue()
    {
        var range1 = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 1, 15));
        var range2 = new DateRange(new DateTime(2024, 1, 10), new DateTime(2024, 1, 20));

        Assert.True(range1.Overlaps(range2));
        Assert.True(range2.Overlaps(range1));
    }

    [Fact]
    public void Overlaps_WithNonOverlappingRange_ReturnsFalse()
    {
        var range1 = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 1, 15));
        var range2 = new DateRange(new DateTime(2024, 1, 20), new DateTime(2024, 1, 31));

        Assert.False(range1.Overlaps(range2));
        Assert.False(range2.Overlaps(range1));
    }

    [Fact]
    public void Overlaps_WithAdjacentRange_ReturnsTrue()
    {
        var range1 = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 1, 15));
        var range2 = new DateRange(new DateTime(2024, 1, 15), new DateTime(2024, 1, 31));

        Assert.True(range1.Overlaps(range2));
    }

    [Fact]
    public void Overlaps_WithNull_ThrowsArgumentNullException()
    {
        var range = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));

        Assert.Throws<ArgumentNullException>(() => range.Overlaps(null!));
    }

    [Fact]
    public void IsWithin_WhenCompletelyInside_ReturnsTrue()
    {
        var inner = new DateRange(new DateTime(2024, 1, 10), new DateTime(2024, 1, 20));
        var outer = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));

        Assert.True(inner.IsWithin(outer));
    }

    [Fact]
    public void IsWithin_WhenPartiallyOutside_ReturnsFalse()
    {
        var range1 = new DateRange(new DateTime(2024, 1, 10), new DateTime(2024, 2, 5));
        var range2 = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));

        Assert.False(range1.IsWithin(range2));
    }

    [Fact]
    public void IsWithin_WhenSameRange_ReturnsTrue()
    {
        var range1 = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));
        var range2 = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));

        Assert.True(range1.IsWithin(range2));
    }

    [Fact]
    public void IsWithin_WithNull_ThrowsArgumentNullException()
    {
        var range = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));

        Assert.Throws<ArgumentNullException>(() => range.IsWithin(null!));
    }

    [Fact]
    public void Today_CreatesRangeForCurrentDay()
    {
        var today = DateTime.UtcNow.Date;
        var range = DateRange.Today();

        Assert.Equal(today, range.StartDate);
        Assert.Equal(today, range.EndDate);
        Assert.Equal(1, range.DurationInDays);
    }

    [Fact]
    public void Last30Days_Creates30DayRange()
    {
        var range = DateRange.Last30Days();

        Assert.Equal(30, range.DurationInDays);
        Assert.Equal(DateTime.UtcNow.Date, range.EndDate);
    }

    [Fact]
    public void Last90Days_Creates90DayRange()
    {
        var range = DateRange.Last90Days();

        Assert.Equal(90, range.DurationInDays);
        Assert.Equal(DateTime.UtcNow.Date, range.EndDate);
    }

    [Fact]
    public void ThisMonth_CreatesCurrentMonthRange()
    {
        var range = DateRange.ThisMonth();
        var today = DateTime.UtcNow.Date;

        Assert.Equal(1, range.StartDate.Day);
        Assert.Equal(today.Month, range.StartDate.Month);
        Assert.True(range.Contains(today));
    }

    [Fact]
    public void ThisYear_CreatesCurrentYearRange()
    {
        var range = DateRange.ThisYear();
        var today = DateTime.UtcNow.Date;

        Assert.Equal(new DateTime(today.Year, 1, 1), range.StartDate);
        Assert.Equal(new DateTime(today.Year, 12, 31), range.EndDate);
    }

    [Fact]
    public void Equals_SameDates_ReturnsTrue()
    {
        var range1 = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));
        var range2 = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));

        Assert.True(range1.Equals(range2));
        Assert.True(range1 == range2);
    }

    [Fact]
    public void Equals_DifferentDates_ReturnsFalse()
    {
        var range1 = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));
        var range2 = new DateRange(new DateTime(2024, 2, 1), new DateTime(2024, 2, 28));

        Assert.False(range1.Equals(range2));
        Assert.True(range1 != range2);
    }

    [Fact]
    public void ToString_FormatsCorrectly()
    {
        var range = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 12, 31));

        Assert.Equal("2024-01-01 to 2024-12-31", range.ToString());
    }

    [Fact]
    public void GetHashCode_SameValues_ReturnsSameHashCode()
    {
        var range1 = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));
        var range2 = new DateRange(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31));

        Assert.Equal(range1.GetHashCode(), range2.GetHashCode());
    }
}
