
namespace Livena.Identity.Api.ApiModels.User;

public class UpdateUserApiInput
{
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime? Birthday { get; set; }
    public bool? IsActive { get; set; }
    
    public UpdateUserApiInput(
        string? email = null,
        string? phone = null,
        bool? isActive = null,
        DateTime? birthday = null)
    {
        Email = email;
        Phone = phone;
        Birthday = birthday;
        IsActive = isActive;
    }
}