using FinTrackPro.Domain.Enums;

namespace FinTrackPro.Domain.ValueObjects;

public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; }

    private Money()
    {
    }

    public Money(decimal amount, Currency currency)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));
        }

        Amount = amount;
        Currency = currency;
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException($"Cannot add money with different currencies: {Currency} and {other.Currency}");
        }

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException($"Cannot subtract money with different currencies: {Currency} and {other.Currency}");
        }

        if (Amount < other.Amount)
        {
            throw new InvalidOperationException("Resulting amount cannot be negative.");
        }

        return new Money(Amount - other.Amount, Currency);
    }

    public static Money Zero(Currency currency) => new Money(0, currency);

    public bool Equals(Money? other)
    {
        if (other is null) return false;
        return Amount == other.Amount && Currency == other.Currency;
    }

    public override bool Equals(object? obj) => Equals(obj as Money);

    public override int GetHashCode() => HashCode.Combine(Amount, Currency);

    public static bool operator ==(Money? left, Money? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Money? left, Money? right) => !(left == right);

    public static bool operator >(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new InvalidOperationException($"Cannot compare money with different currencies: {left.Currency} and {right.Currency}");
        }

        return left.Amount > right.Amount;
    }

    public static bool operator <(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new InvalidOperationException($"Cannot compare money with different currencies: {left.Currency} and {right.Currency}");
        }

        return left.Amount < right.Amount;
    }

    public static bool operator >=(Money left, Money right) => left > right || left == right;

    public static bool operator <=(Money left, Money right) => left < right || left == right;

    public override string ToString() => $"{Amount:N2} {Currency}";
}
