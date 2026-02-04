using FinTrackPro.Domain.Enums;
using FinTrackPro.Domain.ValueObjects;

namespace FinTrackPro.Domain.Entities;

public class RecurringTransaction
{
    public Guid Id { get; private set; }
    public RecurringTransactionType RecurringTransactionType { get; private set; }
    public Money Amount { get; private set; }
    public string Description { get; private set; }
    public Guid AccountId { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime StartDate { get; private set; }
    public RecurrencePeriod RecurrencePeriod { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public bool IsIncome => RecurringTransactionType == RecurringTransactionType.Income;
    public bool IsExpense => RecurringTransactionType == RecurringTransactionType.Expense;

    private RecurringTransaction()
    {
        Description = string.Empty;
        Amount = null!;
        RecurrencePeriod = null!;
    }

    public RecurringTransaction(
        Guid id,
        RecurringTransactionType recurringTransactionType,
        Money amount,
        string description,
        Guid accountId,
        Guid categoryId,
        Guid userId,
        DateTime startDate,
        RecurrencePeriod recurrencePeriod)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Recurring transaction id cannot be empty.", nameof(id));
        }

        if (accountId == Guid.Empty)
        {
            throw new ArgumentException("Account id cannot be empty.", nameof(accountId));
        }

        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("Category id cannot be empty.", nameof(categoryId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        ValidateAmount(amount);
        ValidateDescription(description);

        if (recurrencePeriod == null)
        {
            throw new ArgumentNullException(nameof(recurrencePeriod), "Recurrence period cannot be null.");
        }

        Id = id;
        RecurringTransactionType = recurringTransactionType;
        Amount = amount;
        Description = description.Trim();
        AccountId = accountId;
        CategoryId = categoryId;
        UserId = userId;
        StartDate = startDate.Date;
        RecurrencePeriod = recurrencePeriod;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public static RecurringTransaction CreateIncome(
        Money amount,
        string description,
        Guid accountId,
        Guid categoryId,
        Guid userId,
        DateTime startDate,
        RecurrencePeriod recurrencePeriod)
    {
        if (amount.Amount <= 0)
        {
            throw new ArgumentException("Income amount must be positive.", nameof(amount));
        }

        return new RecurringTransaction(
            Guid.NewGuid(),
            RecurringTransactionType.Income,
            amount,
            description,
            accountId,
            categoryId,
            userId,
            startDate,
            recurrencePeriod);
    }

    public static RecurringTransaction CreateExpense(
        Money amount,
        string description,
        Guid accountId,
        Guid categoryId,
        Guid userId,
        DateTime startDate,
        RecurrencePeriod recurrencePeriod)
    {
        if (amount.Amount <= 0)
        {
            throw new ArgumentException("Expense amount must be positive.", nameof(amount));
        }

        return new RecurringTransaction(
            Guid.NewGuid(),
            RecurringTransactionType.Expense,
            amount,
            description,
            accountId,
            categoryId,
            userId,
            startDate,
            recurrencePeriod);
    }

    public void UpdateDescription(string newDescription)
    {
        ValidateDescription(newDescription);

        Description = newDescription.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAmount(Money newAmount)
    {
        ValidateAmount(newAmount);

        Amount = newAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRecurrencePeriod(RecurrencePeriod newRecurrencePeriod)
    {
        if (newRecurrencePeriod == null)
        {
            throw new ArgumentNullException(nameof(newRecurrencePeriod), "Recurrence period cannot be null.");
        }

        RecurrencePeriod = newRecurrencePeriod;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStartDate(DateTime newStartDate)
    {
        StartDate = newStartDate.Date;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCategory(Guid newCategoryId)
    {
        if (newCategoryId == Guid.Empty)
        {
            throw new ArgumentException("Category id cannot be empty.", nameof(newCategoryId));
        }

        CategoryId = newCategoryId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
        {
            throw new InvalidOperationException("Recurring transaction is already active.");
        }

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Recurring transaction is already deactivated.");
        }

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public DateTime CalculateNextOccurrence(DateTime fromDate)
    {
        if (fromDate < StartDate)
        {
            return StartDate;
        }

        return RecurrencePeriod.CalculateNextOccurrence(fromDate);
    }

    public IEnumerable<DateTime> GetOccurrencesBetween(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
        {
            throw new ArgumentException("Start date must be before or equal to end date.", nameof(startDate));
        }

        var occurrences = new List<DateTime>();
        var currentDate = StartDate;

        while (currentDate <= endDate)
        {
            if (currentDate >= startDate)
            {
                occurrences.Add(currentDate);
            }

            currentDate = RecurrencePeriod.CalculateNextOccurrence(currentDate);
        }

        return occurrences;
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Recurring transaction description cannot be empty.", nameof(description));
        }

        if (description.Length > 500)
        {
            throw new ArgumentException("Recurring transaction description cannot exceed 500 characters.", nameof(description));
        }
    }

    private static void ValidateAmount(Money amount)
    {
        if (amount == null)
        {
            throw new ArgumentNullException(nameof(amount), "Amount cannot be null.");
        }

        if (amount.Amount <= 0)
        {
            throw new ArgumentException("Recurring transaction amount must be positive.", nameof(amount));
        }
    }
}
