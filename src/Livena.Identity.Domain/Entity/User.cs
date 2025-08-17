using Livena.Identity.Domain.Enum;
using Livena.Identity.Domain.Shared;
using Livena.Identity.Domain.Validator;

namespace Livena.Identity.Domain.Entity;

public class User : AggregateRoot
{
    public string Username { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string Password { get; private set; }
    public DateTime Birthday { get; private set; }
    public Roles Role { get; private set; }
    public bool? IsVerified { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public User(
        string username,
        string? email,
        string? phone,
        string password,
        DateTime birthday,
        Roles role = Roles.User,
        bool? isVerified = false,
        bool isActive = true
    )
        : base()
    {
        Username = username;
        Email = email ?? null;
        Phone = phone ?? null;
        Password = password;
        Birthday = DateTimeUtils.EnsureUtc(birthday);
        Role = role;
        IsVerified = isVerified;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void Update(string? email, string? phone, DateTime? birthday, bool? isActive)
    {
        Email = email ?? Email;
        Phone = phone ?? Phone;
        Birthday = birthday.HasValue ? DateTimeUtils.EnsureUtc(birthday.Value) : Birthday;
        IsActive = isActive ?? IsActive;

        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void ChangePassword(string password)
    {
        Password = password;

        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Verify()
    {
        IsVerified = true;
    }

    public void Unverify()
    {
        IsVerified = false;
    }

    private void Validate()
    {
        UserValidator.Validate(Username, Email, Password);
    }
}
