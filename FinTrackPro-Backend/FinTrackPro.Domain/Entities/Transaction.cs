using FinTrackPro.Domain.Enums;
using FinTrackPro.Domain.ValueObjects;

namespace FinTrackPro.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public Money Amount { get; private set; }
    public DateTime Date { get; private set; }
    public string Description { get; private set; }
    public Guid AccountId { get; private set; }
    public Guid UserId { get; private set; }
    //transfers between accounts do not have a category, and other transfers do not have ToAccounts
    public Guid? CategoryId { get; private set; }
    public Guid? ToAccountId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private Transaction()
    {
        Description = string.Empty;
        Amount = null!;
    }

    public Transaction(
        Guid id,
        TransactionType transactionType,
        Money amount,
        DateTime date,
        string description,
        Guid accountId,
        Guid userId,
        Guid? categoryId = null,
        Guid? toAccountId = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Transaction id cannot be empty.", nameof(id));
        }

        if (accountId == Guid.Empty)
        {
            throw new ArgumentException("Account id cannot be empty.", nameof(accountId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        ValidateDescription(description);
        ValidateTransactionRules(transactionType, amount, categoryId, toAccountId);

        Id = id;
        TransactionType = transactionType;
        Amount = amount;
        Date = date;
        Description = description.Trim();
        AccountId = accountId;
        UserId = userId;
        CategoryId = categoryId;
        ToAccountId = toAccountId;
        CreatedAt = DateTime.UtcNow;
    }

    public static Transaction CreateIncome(
        Money amount,
        DateTime date,
        string description,
        Guid accountId,
        Guid userId,
        Guid categoryId)
    {
        if (amount.Amount <= 0)
        {
            throw new ArgumentException("Income amount must be positive.", nameof(amount));
        }

        return new Transaction(
            Guid.NewGuid(),
            TransactionType.Income,
            amount,
            date,
            description,
            accountId,
            userId,
            categoryId,
            toAccountId: null);
    }

    public static Transaction CreateExpense(
        Money amount,
        DateTime date,
        string description,
        Guid accountId,
        Guid userId,
        Guid categoryId)
    {
        if (amount.Amount <= 0)
        {
            throw new ArgumentException("Expense amount must be positive.", nameof(amount));
        }

        return new Transaction(
            Guid.NewGuid(),
            TransactionType.Expense,
            amount,
            date,
            description,
            accountId,
            userId,
            categoryId,
            toAccountId: null);
    }

    public static Transaction CreateTransfer(
        Money amount,
        DateTime date,
        string description,
        Guid fromAccountId,
        Guid toAccountId,
        Guid userId)
    {
        if (amount.Amount <= 0)
        {
            throw new ArgumentException("Transfer amount must be positive.", nameof(amount));
        }

        if (fromAccountId == toAccountId)
        {
            throw new ArgumentException("Cannot transfer to the same account.", nameof(toAccountId));
        }

        return new Transaction(
            Guid.NewGuid(),
            TransactionType.Transfer,
            amount,
            date,
            description,
            fromAccountId,
            userId,
            categoryId: null,
            toAccountId: toAccountId);
    }

    public void UpdateDescription(string newDescription)
    {
        ValidateDescription(newDescription);

        Description = newDescription.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException("Transaction is already deleted.");
        }

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        if (!IsDeleted)
        {
            throw new InvalidOperationException("Transaction is not deleted.");
        }

        IsDeleted = false;
        DeletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsTransfer => TransactionType == TransactionType.Transfer;

    public bool IsIncome => TransactionType == TransactionType.Income;

    public bool IsExpense => TransactionType == TransactionType.Expense;

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Transaction description cannot be empty.", nameof(description));
        }

        if (description.Length > 500)
        {
            throw new ArgumentException("Transaction description cannot exceed 500 characters.", nameof(description));
        }
    }

    private static void ValidateTransactionRules(
        TransactionType transactionType,
        Money amount,
        Guid? categoryId,
        Guid? toAccountId)
    {
        if (amount.Amount <= 0)
        {
            throw new ArgumentException("Transaction amount must be positive.", nameof(amount));
        }

        switch (transactionType)
        {
            case TransactionType.Income:
            case TransactionType.Expense:
                if (!categoryId.HasValue)
                {
                    throw new ArgumentException(
                        $"{transactionType} transactions must have a category.",
                        nameof(categoryId));
                }

                if (toAccountId.HasValue)
                {
                    throw new ArgumentException(
                        $"{transactionType} transactions cannot have a destination account.",
                        nameof(toAccountId));
                }
                break;

            case TransactionType.Transfer:
                if (categoryId.HasValue)
                {
                    throw new ArgumentException(
                        "Transfer transactions cannot have a category.",
                        nameof(categoryId));
                }

                if (!toAccountId.HasValue)
                {
                    throw new ArgumentException(
                        "Transfer transactions must have a destination account.",
                        nameof(toAccountId));
                }

                if (toAccountId.Value == Guid.Empty)
                {
                    throw new ArgumentException(
                        "Destination account id cannot be empty.",
                        nameof(toAccountId));
                }
                break;

            default:
                throw new ArgumentException($"Invalid transaction type: {transactionType}", nameof(transactionType));
        }
    }
}
