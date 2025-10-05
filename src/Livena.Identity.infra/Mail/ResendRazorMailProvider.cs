using Livena.Identity.Application.Interfaces;
using Microsoft.Extensions.Logging;
using RazorLight;
using RazorLight.Compilation;
using Resend;

namespace Livena.Identity.Infra.Mail;

public class ResendRazorMailProvider : IMailProvider
{
    private readonly IResend _resend;
    private readonly string _from;
    private readonly ILogger<ResendRazorMailProvider>? _logger;
    private readonly RazorLightEngine _razor;

    public ResendRazorMailProvider(
        IResend resend,
        string from,
        ILogger<ResendRazorMailProvider>? logger = null
    )
    {
        _resend = resend ?? throw new ArgumentNullException(nameof(resend));
        _from = from ?? throw new ArgumentNullException(nameof(from));
        _logger = logger;

        _razor = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(
                typeof(ResendRazorMailProvider).Assembly,
                "Livena.Identity.infra.Mail.Templates"
            )
            .UseMemoryCachingProvider()
            .Build();
    }

    public async Task SendAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var message = new EmailMessage
            {
                From = _from,
                To = { to },
                Subject = subject,
                HtmlBody = body,
            };

            await _resend.EmailSendAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error sending email to {To}", to);
            throw;
        }
    }

    public async Task SendFromTemplateAsync<TModel>(
        string to,
        string subject,
        string templateKey,
        TModel model,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var html = await _razor.CompileRenderAsync(templateKey, model);
            await SendAsync(to, subject, html, cancellationToken);
        }
        catch (TemplateCompilationException tex)
        {
            _logger?.LogError(tex, "Error compiling template {TemplateKey}", templateKey);
            throw;
        }
    }
}
