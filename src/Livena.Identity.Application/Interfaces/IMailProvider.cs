namespace Livena.Identity.Application.Interfaces;

public interface IMailProvider
{
    Task SendAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default
    );

    Task SendFromTemplateAsync<TModel>(
        string to,
        string subject,
        string templateKey,
        TModel model,
        CancellationToken cancellationToken = default
    );
}
