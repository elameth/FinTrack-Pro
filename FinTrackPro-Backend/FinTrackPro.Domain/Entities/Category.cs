namespace FinTrackPro.Domain.Entities;

public class Category
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    public string? Icon { get; private set; }
    public string? Color { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public bool IsSubCategory => ParentCategoryId.HasValue;

    private Category()
    {
        Name = string.Empty;
    }

    public Category(
        Guid id,
        string name,
        Guid userId,
        string? description = null,
        Guid? parentCategoryId = null,
        string? icon = null,
        string? color = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Category id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        ValidateName(name);
        ValidateDescription(description);
        ValidateColor(color);

        Id = id;
        Name = name.Trim();
        Description = description?.Trim();
        UserId = userId;
        ParentCategoryId = parentCategoryId;
        Icon = icon?.Trim();
        Color = color?.Trim();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public static Category Create(
        string name,
        Guid userId,
        string? description = null,
        Guid? parentCategoryId = null,
        string? icon = null,
        string? color = null)
    {
        return new Category(
            Guid.NewGuid(),
            name,
            userId,
            description,
            parentCategoryId,
            icon,
            color);
    }

    public void UpdateName(string newName)
    {
        ValidateName(newName);

        Name = newName.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string? newDescription)
    {
        ValidateDescription(newDescription);

        Description = newDescription?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetParentCategory(Guid? parentCategoryId)
    {
        if (parentCategoryId.HasValue && parentCategoryId.Value == Id)
        {
            throw new ArgumentException("Category cannot be its own parent.", nameof(parentCategoryId));
        }

        ParentCategoryId = parentCategoryId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
        {
            throw new InvalidOperationException("Category is already active.");
        }

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Category is already deactivated.");
        }

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateIcon(string? newIcon)
    {
        Icon = newIcon?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateColor(string? newColor)
    {
        ValidateColor(newColor);

        Color = newColor?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAppearance(string? newIcon, string? newColor)
    {
        ValidateColor(newColor);

        Icon = newIcon?.Trim();
        Color = newColor?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name cannot be empty.", nameof(name));
        }

        if (name.Length > 100)
        {
            throw new ArgumentException("Category name cannot exceed 100 characters.", nameof(name));
        }

        if (name.Length < 2)
        {
            throw new ArgumentException("Category name must be at least 2 characters.", nameof(name));
        }
    }

    private static void ValidateDescription(string? description)
    {
        if (description != null && description.Length > 500)
        {
            throw new ArgumentException("Category description cannot exceed 500 characters.", nameof(description));
        }
    }

    private static void ValidateColor(string? color)
    {
        if (color != null && !string.IsNullOrWhiteSpace(color))
        {
            if (!color.StartsWith('#') || (color.Length != 7 && color.Length != 4))
            {
                throw new ArgumentException(
                    "Color must be a valid hex color code (e.g., #FF5733 or #F73).",
                    nameof(color));
            }
        }
    }
}
