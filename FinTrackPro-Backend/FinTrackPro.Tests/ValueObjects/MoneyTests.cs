using FinTrackPro.Domain.Enums;
using FinTrackPro.Domain.ValueObjects;

namespace FinTrackPro.Tests.ValueObjects;

public class MoneyTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidAmount_ShouldCreateMoney()
    {
        // Arrange & Act
        var money = new Money(100.50m, Currency.USD);

        // Assert
        Assert.Equal(100.50m, money.Amount);
        Assert.Equal(Currency.USD, money.Currency);
    }

    [Fact]
    public void Constructor_WithZeroAmount_ShouldCreateMoney()
    {
        // Arrange & Act
        var money = new Money(0, Currency.EUR);

        // Assert
        Assert.Equal(0, money.Amount);
        Assert.Equal(Currency.EUR, money.Currency);
    }

    [Fact]
    public void Constructor_WithNegativeAmount_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Money(-10.00m, Currency.USD));
        Assert.Equal("Amount cannot be negative. (Parameter 'amount')", exception.Message);
    }

    [Fact]
    public void Constructor_WithDifferentCurrencies_ShouldCreateDistinctMoney()
    {
        // Arrange & Act
        var usdMoney = new Money(100, Currency.USD);
        var eurMoney = new Money(100, Currency.EUR);

        // Assert
        Assert.NotEqual(usdMoney, eurMoney);
    }

    #endregion

    #region Add Method Tests

    [Fact]
    public void Add_WithSameCurrency_ShouldReturnSummedMoney()
    {
        // Arrange
        var money1 = new Money(100.50m, Currency.USD);
        var money2 = new Money(50.25m, Currency.USD);

        // Act
        var result = money1.Add(money2);

        // Assert
        Assert.Equal(150.75m, result.Amount);
        Assert.Equal(Currency.USD, result.Currency);
    }

    [Fact]
    public void Add_WithDifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var usdMoney = new Money(100, Currency.USD);
        var eurMoney = new Money(50, Currency.EUR);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => usdMoney.Add(eurMoney));
        Assert.Contains("Cannot add money with different currencies", exception.Message);
    }

    [Fact]
    public void Add_WithZeroAmount_ShouldReturnOriginalAmount()
    {
        // Arrange
        var money = new Money(100, Currency.USD);
        var zero = Money.Zero(Currency.USD);

        // Act
        var result = money.Add(zero);

        // Assert
        Assert.Equal(100, result.Amount);
        Assert.Equal(Currency.USD, result.Currency);
    }

    [Fact]
    public void Add_LargeAmounts_ShouldHandleCorrectly()
    {
        // Arrange
        var money1 = new Money(999999999.99m, Currency.USD);
        var money2 = new Money(0.01m, Currency.USD);

        // Act
        var result = money1.Add(money2);

        // Assert
        Assert.Equal(1000000000.00m, result.Amount);
    }

    #endregion

    #region Subtract Method Tests

    [Fact]
    public void Subtract_WithSameCurrency_ShouldReturnDifference()
    {
        // Arrange
        var money1 = new Money(100.75m, Currency.USD);
        var money2 = new Money(50.25m, Currency.USD);

        // Act
        var result = money1.Subtract(money2);

        // Assert
        Assert.Equal(50.50m, result.Amount);
        Assert.Equal(Currency.USD, result.Currency);
    }

    [Fact]
    public void Subtract_WithDifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var usdMoney = new Money(100, Currency.USD);
        var eurMoney = new Money(50, Currency.EUR);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => usdMoney.Subtract(eurMoney));
        Assert.Contains("Cannot subtract money with different currencies", exception.Message);
    }

    [Fact]
    public void Subtract_ResultingInNegative_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var money1 = new Money(50, Currency.USD);
        var money2 = new Money(100, Currency.USD);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => money1.Subtract(money2));
        Assert.Equal("Resulting amount cannot be negative.", exception.Message);
    }

    [Fact]
    public void Subtract_EqualAmounts_ShouldReturnZero()
    {
        // Arrange
        var money1 = new Money(100, Currency.USD);
        var money2 = new Money(100, Currency.USD);

        // Act
        var result = money1.Subtract(money2);

        // Assert
        Assert.Equal(0, result.Amount);
        Assert.Equal(Currency.USD, result.Currency);
    }

    [Fact]
    public void Subtract_SubtractingZero_ShouldReturnOriginalAmount()
    {
        // Arrange
        var money = new Money(100, Currency.USD);
        var zero = Money.Zero(Currency.USD);

        // Act
        var result = money.Subtract(zero);

        // Assert
        Assert.Equal(100, result.Amount);
    }

    #endregion

    #region Zero Factory Method Tests

    [Fact]
    public void Zero_ShouldCreateMoneyWithZeroAmount()
    {
        // Act
        var zero = Money.Zero(Currency.USD);

        // Assert
        Assert.Equal(0, zero.Amount);
        Assert.Equal(Currency.USD, zero.Currency);
    }

    [Theory]
    [InlineData(Currency.USD)]
    [InlineData(Currency.EUR)]
    [InlineData(Currency.GBP)]
    public void Zero_WithDifferentCurrencies_ShouldCreateZeroMoney(Currency currency)
    {
        // Act
        var zero = Money.Zero(currency);

        // Assert
        Assert.Equal(0, zero.Amount);
        Assert.Equal(currency, zero.Currency);
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_SameAmountAndCurrency_ShouldReturnTrue()
    {
        // Arrange
        var money1 = new Money(100, Currency.USD);
        var money2 = new Money(100, Currency.USD);

        // Act & Assert
        Assert.True(money1.Equals(money2));
        Assert.True(money1 == money2);
        Assert.False(money1 != money2);
    }

    [Fact]
    public void Equals_DifferentAmounts_ShouldReturnFalse()
    {
        // Arrange
        var money1 = new Money(100, Currency.USD);
        var money2 = new Money(50, Currency.USD);

        // Act & Assert
        Assert.False(money1.Equals(money2));
        Assert.False(money1 == money2);
        Assert.True(money1 != money2);
    }

    [Fact]
    public void Equals_DifferentCurrencies_ShouldReturnFalse()
    {
        // Arrange
        var money1 = new Money(100, Currency.USD);
        var money2 = new Money(100, Currency.EUR);

        // Act & Assert
        Assert.False(money1.Equals(money2));
        Assert.False(money1 == money2);
        Assert.True(money1 != money2);
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var money = new Money(100, Currency.USD);

        // Act & Assert
        Assert.False(money.Equals(null));
        Assert.False(money == null);
        Assert.True(money != null);
    }

    [Fact]
    public void Equals_BothNull_ShouldReturnTrue()
    {
        // Arrange
        Money? money1 = null;
        Money? money2 = null;

        // Act & Assert
        Assert.True(money1 == money2);
        Assert.False(money1 != money2);
    }

    [Fact]
    public void Equals_WithSameReference_ShouldReturnTrue()
    {
        // Arrange
        var money = new Money(100, Currency.USD);

        // Act & Assert
        Assert.True(money.Equals(money));
    }

    #endregion

    #region Comparison Operator Tests

    [Fact]
    public void GreaterThan_WhenLeftIsGreater_ShouldReturnTrue()
    {
        // Arrange
        var money1 = new Money(100, Currency.USD);
        var money2 = new Money(50, Currency.USD);

        // Act & Assert
        Assert.True(money1 > money2);
        Assert.False(money2 > money1);
    }

    [Fact]
    public void GreaterThan_WithDifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var usdMoney = new Money(100, Currency.USD);
        var eurMoney = new Money(50, Currency.EUR);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => usdMoney > eurMoney);
        Assert.Contains("Cannot compare money with different currencies", exception.Message);
    }

    [Fact]
    public void LessThan_WhenLeftIsLess_ShouldReturnTrue()
    {
        // Arrange
        var money1 = new Money(50, Currency.USD);
        var money2 = new Money(100, Currency.USD);

        // Act & Assert
        Assert.True(money1 < money2);
        Assert.False(money2 < money1);
    }

    [Fact]
    public void LessThan_WithDifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var usdMoney = new Money(100, Currency.USD);
        var eurMoney = new Money(50, Currency.EUR);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => usdMoney < eurMoney);
        Assert.Contains("Cannot compare money with different currencies", exception.Message);
    }

    [Fact]
    public void GreaterThanOrEqual_WhenEqual_ShouldReturnTrue()
    {
        // Arrange
        var money1 = new Money(100, Currency.USD);
        var money2 = new Money(100, Currency.USD);

        // Act & Assert
        Assert.True(money1 >= money2);
        Assert.True(money2 >= money1);
    }

    [Fact]
    public void GreaterThanOrEqual_WhenGreater_ShouldReturnTrue()
    {
        // Arrange
        var money1 = new Money(100, Currency.USD);
        var money2 = new Money(50, Currency.USD);

        // Act & Assert
        Assert.True(money1 >= money2);
        Assert.False(money2 >= money1);
    }

    [Fact]
    public void LessThanOrEqual_WhenEqual_ShouldReturnTrue()
    {
        // Arrange
        var money1 = new Money(100, Currency.USD);
        var money2 = new Money(100, Currency.USD);

        // Act & Assert
        Assert.True(money1 <= money2);
        Assert.True(money2 <= money1);
    }

    [Fact]
    public void LessThanOrEqual_WhenLess_ShouldReturnTrue()
    {
        // Arrange
        var money1 = new Money(50, Currency.USD);
        var money2 = new Money(100, Currency.USD);

        // Act & Assert
        Assert.True(money1 <= money2);
        Assert.False(money2 <= money1);
    }

    [Fact]
    public void Comparison_EqualAmounts_ShouldNotBeGreaterOrLess()
    {
        // Arrange
        var money1 = new Money(100, Currency.USD);
        var money2 = new Money(100, Currency.USD);

        // Act & Assert
        Assert.False(money1 > money2);
        Assert.False(money1 < money2);
        Assert.True(money1 >= money2);
        Assert.True(money1 <= money2);
    }

    #endregion

    #region HashCode Tests

    [Fact]
    public void GetHashCode_SameValues_ShouldReturnSameHashCode()
    {
        // Arrange
        var money1 = new Money(100, Currency.USD);
        var money2 = new Money(100, Currency.USD);

        // Act & Assert
        Assert.Equal(money1.GetHashCode(), money2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentAmounts_ShouldReturnDifferentHashCode()
    {
        // Arrange
        var money1 = new Money(100, Currency.USD);
        var money2 = new Money(50, Currency.USD);

        // Act & Assert
        Assert.NotEqual(money1.GetHashCode(), money2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentCurrencies_ShouldReturnDifferentHashCode()
    {
        // Arrange
        var money1 = new Money(100, Currency.USD);
        var money2 = new Money(100, Currency.EUR);

        // Act & Assert
        Assert.NotEqual(money1.GetHashCode(), money2.GetHashCode());
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var money = new Money(1234.56m, Currency.USD);

        // Act
        var result = money.ToString();

        // Assert
        Assert.Equal("1,234.56 USD", result);
    }

    [Fact]
    public void ToString_WithZeroAmount_ShouldFormatCorrectly()
    {
        // Arrange
        var money = Money.Zero(Currency.EUR);

        // Act
        var result = money.ToString();

        // Assert
        Assert.Equal("0.00 EUR", result);
    }

    [Fact]
    public void ToString_WithLargeAmount_ShouldFormatWithCommas()
    {
        // Arrange
        var money = new Money(1000000.99m, Currency.GBP);

        // Act
        var result = money.ToString();

        // Assert
        Assert.Equal("1,000,000.99 GBP", result);
    }

    [Theory]
    [InlineData(0.01, "0.01")]
    [InlineData(10, "10.00")]
    [InlineData(100.5, "100.50")]
    [InlineData(1000.99, "1,000.99")]
    public void ToString_VariousAmounts_ShouldFormatCorrectly(decimal amount, string expectedFormat)
    {
        // Arrange
        var money = new Money(amount, Currency.USD);

        // Act
        var result = money.ToString();

        // Assert
        Assert.Equal($"{expectedFormat} USD", result);
    }

    #endregion

    #region Edge Cases and Integration Tests

    [Fact]
    public void Money_WithVerySmallAmount_ShouldHandleCorrectly()
    {
        // Arrange & Act
        var money = new Money(0.01m, Currency.USD);

        // Assert
        Assert.Equal(0.01m, money.Amount);
    }

    [Fact]
    public void Money_MultipleOperations_ShouldMaintainImmutability()
    {
        // Arrange
        var original = new Money(100, Currency.USD);
        var toAdd = new Money(50, Currency.USD);
        var toSubtract = new Money(25, Currency.USD);

        // Act
        var afterAdd = original.Add(toAdd);
        var afterSubtract = afterAdd.Subtract(toSubtract);

        // Assert
        Assert.Equal(100, original.Amount); // Original unchanged
        Assert.Equal(150, afterAdd.Amount);
        Assert.Equal(125, afterSubtract.Amount);
    }

    [Fact]
    public void Money_ChainedOperations_ShouldCalculateCorrectly()
    {
        // Arrange
        var money = new Money(1000, Currency.USD);

        // Act
        var result = money
            .Add(new Money(500, Currency.USD))
            .Subtract(new Money(300, Currency.USD))
            .Add(new Money(100, Currency.USD));

        // Assert
        Assert.Equal(1300, result.Amount);
        Assert.Equal(Currency.USD, result.Currency);
    }
    
    [Fact]
    public void Money_DecimalPrecision_ShouldMaintainAccuracy()
    {
        // Arrange
        var money1 = new Money(10.10m, Currency.USD);
        var money2 = new Money(20.20m, Currency.USD);

        // Act
        var sum = money1.Add(money2);

        // Assert
        Assert.Equal(30.30m, sum.Amount);
    }

    #endregion
}
