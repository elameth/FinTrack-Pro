using FinTrackPro.Domain.Enums;

namespace FinTrackPro.Domain.ValueObjects;

public sealed class RecurrencePeriod : IEquatable<RecurrencePeriod>
{
    public RecurrenceType RecurrenceType { get; }
    public int Interval { get; }

    public RecurrencePeriod(RecurrenceType recurrenceType, int interval)
    {
        if (interval <= 0)
        {
            throw new ArgumentException("Recurrence interval must be positive.", nameof(interval));
        }

        RecurrenceType = recurrenceType;
        Interval = interval;
    }

    public static RecurrencePeriod Daily(int interval = 1)
        => new RecurrencePeriod(RecurrenceType.Daily, interval);

    public static RecurrencePeriod Weekly(int interval = 1)
        => new RecurrencePeriod(RecurrenceType.Weekly, interval);

    public static RecurrencePeriod Biweekly()
        => new RecurrencePeriod(RecurrenceType.Weekly, 2);

    public static RecurrencePeriod Monthly(int interval = 1)
        => new RecurrencePeriod(RecurrenceType.Monthly, interval);

    public static RecurrencePeriod Quarterly()
        => new RecurrencePeriod(RecurrenceType.Monthly, 3);

    public static RecurrencePeriod Yearly(int interval = 1)
        => new RecurrencePeriod(RecurrenceType.Yearly, interval);

    public DateTime CalculateNextOccurrence(DateTime fromDate)
    {
        return RecurrenceType switch
        {
            RecurrenceType.Daily => fromDate.AddDays(Interval),
            RecurrenceType.Weekly => fromDate.AddDays(Interval * 7),
            RecurrenceType.Monthly => fromDate.AddMonths(Interval),
            RecurrenceType.Yearly => fromDate.AddYears(Interval),
            _ => throw new InvalidOperationException($"Unknown recurrence type: {RecurrenceType}")
        };
    }

    public bool Equals(RecurrencePeriod? other)
    {
        if (other is null) return false;
        return RecurrenceType == other.RecurrenceType && Interval == other.Interval;
    }

    public override bool Equals(object? obj) => Equals(obj as RecurrencePeriod);

    public override int GetHashCode() => HashCode.Combine(RecurrenceType, Interval);

    public static bool operator ==(RecurrencePeriod? left, RecurrencePeriod? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(RecurrencePeriod? left, RecurrencePeriod? right)
        => !(left == right);

    public override string ToString()
    {
        if (Interval == 1)
        {
            return RecurrenceType.ToString();
        }

        return $"Every {Interval} {RecurrenceType.ToString().ToLower()}";
    }
}
