namespace Livena.Identity.Application.UseCases.User.Common;

public class TemplateModel
{
    public string Username { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int ExpireMinutes { get; set; }
}