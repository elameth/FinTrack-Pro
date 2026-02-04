using FinTrackPro.Domain.Enums;
using FinTrackPro.Domain.ValueObjects;

namespace FinTrackPro.Tests.Domain.ValueObjects;

public class MoneyTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(100.50)]
    [InlineData(999999.99)]
    public void Constructor_WithValidAmount_CreatesInstance(decimal amount)
    {
        var money = new Money(amount, Currency.USD);

        Assert.Equal(amount, money.Amount);
        Assert.Equal(Currency.USD, money.Currency);
    }

    [Fact]
    public void Constructor_WithNegativeAmount_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new Money(-10, Currency.USD));
        Assert.Contains("cannot be negative", exception.Message);
    }

    [Theory]
    [InlineData(100, 50, 150)]
    [InlineData(0.01, 0.02, 0.03)]
    [InlineData(999999.99, 0.01, 1000000)]
    public void Add_WithSameCurrency_ReturnsSummedAmount(decimal amount1, decimal amount2, decimal expected)
    {
        var money1 = new Money(amount1, Currency.USD);
        var money2 = new Money(amount2, Currency.USD);

        var result = money1.Add(money2);

        Assert.Equal(expected, result.Amount);
    }

    [Fact]
    public void Add_WithDifferentCurrencies_ThrowsInvalidOperationException()
    {
        var usdMoney = new Money(100, Currency.USD);
        var eurMoney = new Money(50, Currency.EUR);

        Assert.Throws<InvalidOperationException>(() => usdMoney.Add(eurMoney));
    }

    [Theory]
    [InlineData(100, 50, 50)]
    [InlineData(100, 100, 0)]
    [InlineData(150.75, 50.25, 100.50)]
    public void Subtract_WithSameCurrency_ReturnsDifference(decimal amount1, decimal amount2, decimal expected)
    {
        var money1 = new Money(amount1, Currency.USD);
        var money2 = new Money(amount2, Currency.USD);

        var result = money1.Subtract(money2);

        Assert.Equal(expected, result.Amount);
    }

    [Fact]
    public void Subtract_ResultingInNegative_ThrowsInvalidOperationException()
    {
        var money1 = new Money(50, Currency.USD);
        var money2 = new Money(100, Currency.USD);

        Assert.Throws<InvalidOperationException>(() => money1.Subtract(money2));
    }

    [Fact]
    public void Subtract_WithDifferentCurrencies_ThrowsInvalidOperationException()
    {
        var usdMoney = new Money(100, Currency.USD);
        var eurMoney = new Money(50, Currency.EUR);

        Assert.Throws<InvalidOperationException>(() => usdMoney.Subtract(eurMoney));
    }

    [Theory]
    [InlineData(Currency.USD)]
    [InlineData(Currency.EUR)]
    [InlineData(Currency.GBP)]
    public void Zero_CreatesZeroMoney(Currency currency)
    {
        var zero = Money.Zero(currency);

        Assert.Equal(0, zero.Amount);
        Assert.Equal(currency, zero.Currency);
    }

    [Fact]
    public void Equals_SameAmountAndCurrency_ReturnsTrue()
    {
        var money1 = new Money(100, Currency.USD);
        var money2 = new Money(100, Currency.USD);

        Assert.True(money1.Equals(money2));
        Assert.True(money1 == money2);
    }

    [Theory]
    [InlineData(100, Currency.USD, 50, Currency.USD)]
    [InlineData(100, Currency.USD, 100, Currency.EUR)]
    public void Equals_DifferentValues_ReturnsFalse(decimal amount1, Currency currency1, decimal amount2, Currency currency2)
    {
        var money1 = new Money(amount1, currency1);
        var money2 = new Money(amount2, currency2);

        Assert.False(money1.Equals(money2));
        Assert.True(money1 != money2);
    }

    [Fact]
    public void GreaterThan_ComparesAmountsCorrectly()
    {
        var larger = new Money(100, Currency.USD);
        var smaller = new Money(50, Currency.USD);

        Assert.True(larger > smaller);
        Assert.False(smaller > larger);
        Assert.True(larger >= smaller);
        Assert.False(smaller >= larger);
    }

    [Fact]
    public void LessThan_ComparesAmountsCorrectly()
    {
        var larger = new Money(100, Currency.USD);
        var smaller = new Money(50, Currency.USD);

        Assert.True(smaller < larger);
        Assert.False(larger < smaller);
        Assert.True(smaller <= larger);
        Assert.False(larger <= smaller);
    }

    [Fact]
    public void ComparisonOperators_WithDifferentCurrencies_ThrowsInvalidOperationException()
    {
        var usdMoney = new Money(100, Currency.USD);
        var eurMoney = new Money(50, Currency.EUR);

        Assert.Throws<InvalidOperationException>(() => usdMoney > eurMoney);
        Assert.Throws<InvalidOperationException>(() => usdMoney < eurMoney);
    }

    [Fact]
    public void GetHashCode_SameValues_ReturnsSameHashCode()
    {
        var money1 = new Money(100, Currency.USD);
        var money2 = new Money(100, Currency.USD);

        Assert.Equal(money1.GetHashCode(), money2.GetHashCode());
    }

    [Theory]
    [InlineData(1234.56, "1,234.56 USD")]
    [InlineData(0, "0.00 USD")]
    [InlineData(1000000.99, "1,000,000.99 USD")]
    public void ToString_FormatsCorrectly(decimal amount, string expected)
    {
        var money = new Money(amount, Currency.USD);

        Assert.Equal(expected, money.ToString());
    }

    [Fact]
    public void Operations_MaintainImmutability()
    {
        var original = new Money(100, Currency.USD);
        var added = original.Add(new Money(50, Currency.USD));

        Assert.Equal(100, original.Amount);
        Assert.Equal(150, added.Amount);
    }

    [Fact]
    public void ChainedOperations_CalculateCorrectly()
    {
        var result = new Money(1000, Currency.USD)
            .Add(new Money(500, Currency.USD))
            .Subtract(new Money(300, Currency.USD));

        Assert.Equal(1200, result.Amount);
    }
}
