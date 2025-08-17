namespace Livena.Identity.Api.ApiModels.User;

public class ChangePasswordApiInput
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }

    public ChangePasswordApiInput(string currentPassword, string newPassword)
    {
        CurrentPassword = currentPassword;
        NewPassword = newPassword;
    }
}
