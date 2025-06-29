using Livena.Identity.Domain.Enum;

namespace Livena.Identity.Application.UseCases.User.Common;

public record UserOutput
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set;  }
    public DateTime Birthday { get; private set; }
    public Roles Role { get; private set; }
    public bool? IsVerified { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public UserOutput(
        Guid id, 
        string username, 
        string? email, 
        string? phone, 
        DateTime birthday, 
        Roles role, 
        bool? isVerified, 
        bool isActive, 
        DateTime createdAt, 
        DateTime updatedAt
        )
    {
        Id = id;
        Username = username;
        Email = email;
        Phone = phone;
        Birthday = birthday;
        Role = role;
        IsVerified = isVerified;
        IsActive = isActive;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static UserOutput FromUser(Domain.Entity.User user)
        => new(
            user.Id,
            user.Username,
            user.Email,
            user.Phone,
            user.Birthday,
            user.Role,
            user.IsVerified,
            user.IsActive,
            user.CreatedAt,
            user.UpdatedAt
        );
}