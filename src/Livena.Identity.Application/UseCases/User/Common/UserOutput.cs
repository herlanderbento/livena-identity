namespace Livena.Identity.Application.UseCases.User.Common;

public record UserOutput
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime? Birthday { get; set; }
    public bool? IsVerified { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public UserOutput(
        Guid id,
        string username,
        string? email,
        string? phone,
        DateTime? birthday,
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
        IsVerified = isVerified;
        IsActive = isActive;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static UserOutput FromUser(Domain.Entity.User user) =>
        new(
            user.Id,
            user.Username,
            user.Email,
            user.Phone,
            user.Birthday,
            user.IsVerified,
            user.IsActive,
            user.CreatedAt,
            user.UpdatedAt
        );
}
