using System.Text.RegularExpressions;

namespace FinTrackPro.Domain.Entities;

public class User
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public bool IsActive { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    //for EF Core, non-nullable strings are important here
    private User()
    {
        Email = string.Empty;
        PasswordHash = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
    }

    public User(Guid id, string email, string passwordHash, string firstName, string lastName)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(id));
        }

        ValidateEmail(email);
        ValidatePasswordHash(passwordHash);
        ValidateName(firstName, nameof(firstName));
        ValidateName(lastName, nameof(lastName));

        Id = id;
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        IsActive = true;
        EmailConfirmed = false;
        CreatedAt = DateTime.UtcNow;
    }

    public static User Create(string email, string passwordHash, string firstName, string lastName)
    {
        return new User(Guid.NewGuid(), email, passwordHash, firstName, lastName);
    }
    
    public void UpdatePassword(string newPasswordHash)
    {
        ValidatePasswordHash(newPasswordHash);

        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string firstName, string lastName)
    {
        ValidateName(firstName, nameof(firstName));
        ValidateName(lastName, nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfirmEmail()
    {
        if (EmailConfirmed)
        {
            throw new InvalidOperationException("Email is already confirmed.");
        }

        EmailConfirmed = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordLogin()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Cannot record login for an inactive user.");
        }

        if (!EmailConfirmed)
        {
            throw new InvalidOperationException("Cannot record login for a user with unconfirmed email.");
        }

        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty.", nameof(email));
        }

        if (email.Length > 255)
        {
            throw new ArgumentException("Email cannot exceed 255 characters.", nameof(email));
        }

        if (!EmailRegex.IsMatch(email))
        {
            throw new ArgumentException("Email format is invalid.", nameof(email));
        }
    }

    private static void ValidatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));
        }

        if (passwordHash.Length < 32)
        {
            throw new ArgumentException("Password hash appears to be invalid (too short).", nameof(passwordHash));
        }
    }

    private static void ValidateName(string name, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"{parameterName} cannot be empty.", parameterName);
        }

        if (name.Length > 100)
        {
            throw new ArgumentException($"{parameterName} cannot exceed 100 characters.", parameterName);
        }

        if (name.Length < 2)
        {
            throw new ArgumentException($"{parameterName} must be at least 2 characters.", parameterName);
        }
    }
}
