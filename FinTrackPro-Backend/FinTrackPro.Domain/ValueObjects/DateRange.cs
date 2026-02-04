namespace FinTrackPro.Domain.ValueObjects;

public sealed class DateRange : IEquatable<DateRange>
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    public DateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
        {
            throw new ArgumentException(
                $"Start date ({startDate:yyyy-MM-dd}) cannot be after end date ({endDate:yyyy-MM-dd}).",
                nameof(startDate));
        }

        StartDate = startDate.Date;
        EndDate = endDate.Date;
    }

    public TimeSpan Duration => EndDate - StartDate;

    public int DurationInDays => (int)Duration.TotalDays + 1;

    public bool Contains(DateTime date)
    {
        var dateOnly = date.Date;
        return dateOnly >= StartDate && dateOnly <= EndDate;
    }

    public bool Overlaps(DateRange other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        return StartDate <= other.EndDate && EndDate >= other.StartDate;
    }

    public bool IsWithin(DateRange other)
    {
        if (other is null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        return StartDate >= other.StartDate && EndDate <= other.EndDate;
    }

    public static DateRange Today()
    {
        var today = DateTime.UtcNow.Date;
        return new DateRange(today, today);
    }

    public static DateRange Yesterday()
    {
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        return new DateRange(yesterday, yesterday);
    }

    public static DateRange ThisWeek()
    {
        var today = DateTime.UtcNow.Date;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
        var endOfWeek = startOfWeek.AddDays(6);
        return new DateRange(startOfWeek, endOfWeek);
    }

    public static DateRange LastWeek()
    {
        var today = DateTime.UtcNow.Date;
        var startOfLastWeek = today.AddDays(-(int)today.DayOfWeek - 7);
        var endOfLastWeek = startOfLastWeek.AddDays(6);
        return new DateRange(startOfLastWeek, endOfLastWeek);
    }

    public static DateRange ThisMonth()
    {
        var today = DateTime.UtcNow.Date;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
        return new DateRange(startOfMonth, endOfMonth);
    }

    public static DateRange LastMonth()
    {
        var today = DateTime.UtcNow.Date;
        var startOfLastMonth = new DateTime(today.Year, today.Month, 1).AddMonths(-1);
        var endOfLastMonth = startOfLastMonth.AddMonths(1).AddDays(-1);
        return new DateRange(startOfLastMonth, endOfLastMonth);
    }

    public static DateRange ThisYear()
    {
        var today = DateTime.UtcNow.Date;
        var startOfYear = new DateTime(today.Year, 1, 1);
        var endOfYear = new DateTime(today.Year, 12, 31);
        return new DateRange(startOfYear, endOfYear);
    }

    public static DateRange LastYear()
    {
        var today = DateTime.UtcNow.Date;
        var lastYear = today.Year - 1;
        var startOfLastYear = new DateTime(lastYear, 1, 1);
        var endOfLastYear = new DateTime(lastYear, 12, 31);
        return new DateRange(startOfLastYear, endOfLastYear);
    }

    public static DateRange Last30Days()
    {
        var today = DateTime.UtcNow.Date;
        var startDate = today.AddDays(-29);
        return new DateRange(startDate, today);
    }

    public static DateRange Last90Days()
    {
        var today = DateTime.UtcNow.Date;
        var startDate = today.AddDays(-89);
        return new DateRange(startDate, today);
    }

    public bool Equals(DateRange? other)
    {
        if (other is null) return false;
        return StartDate == other.StartDate && EndDate == other.EndDate;
    }

    public override bool Equals(object? obj) => Equals(obj as DateRange);

    public override int GetHashCode() => HashCode.Combine(StartDate, EndDate);

    public static bool operator ==(DateRange? left, DateRange? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    
    public static bool operator !=(DateRange? left, DateRange? right) => !(left == right);

    public override string ToString() => $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}";
}
