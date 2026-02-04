using FinTrackPro.Domain.Enums;
using FinTrackPro.Domain.ValueObjects;

namespace FinTrackPro.Domain.Entities;

public class Account
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public AccountType AccountType { get; private set; }
    public Money Balance { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }
    
    //for EF Core
    private Account()
    {
        Name = string.Empty;
        Balance = null!;
    }

    public Account(Guid id, string name, AccountType accountType, Currency currency, Guid userId)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Account id cannot be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Account name cannot be empty.", nameof(name));
        }

        if (name.Length > 100)
        {
            throw new ArgumentException("Account name cannot exceed 100 characters.", nameof(name));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        Id = id;
        Name = name.Trim();
        AccountType = accountType;
        Balance = Money.Zero(currency);
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static Account Create(string name, AccountType accountType, Currency currency, Guid userId)
    {
        return new Account(Guid.NewGuid(), name, accountType, currency, userId);
    }

    public void Deposit(Money amount)
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Cannot deposit to an inactive account.");
        }

        if (amount.Currency != Balance.Currency)
        {
            throw new InvalidOperationException(
                $"Cannot deposit {amount.Currency} to an account with {Balance.Currency} currency.");
        }

        Balance = Balance.Add(amount);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Withdraw(Money amount)
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Cannot withdraw from an inactive account.");
        }

        if (amount.Currency != Balance.Currency)
        {
            throw new InvalidOperationException(
                $"Cannot withdraw {amount.Currency} from an account with {Balance.Currency} currency.");
        }

        if (AccountType == AccountType.CreditCard)
        {
            Balance = Balance.Subtract(amount);
            UpdatedAt = DateTime.UtcNow;
            return;
        }

        if (Balance < amount)
        {
            throw new InvalidOperationException(
                $"Insufficient funds. Current balance: {Balance}, Withdrawal amount: {amount}");
        }

        Balance = Balance.Subtract(amount);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Account name cannot be empty.", nameof(newName));
        }

        if (newName.Length > 100)
        {
            throw new ArgumentException("Account name cannot exceed 100 characters.", nameof(newName));
        }

        Name = newName.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Account is already deactivated.");
        }

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
        {
            throw new InvalidOperationException("Account is already active.");
        }

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
