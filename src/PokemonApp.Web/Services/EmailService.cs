using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace PokemonApp.Web.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SmtpSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendPokemonReportAsync(
        string toAddress,
        string subject,
        string bodyText,
        byte[] excelAttachment,
        string attachmentFileName,
        CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
        message.To.Add(MailboxAddress.Parse(toAddress));
        message.Subject = subject;

        var builder = new BodyBuilder { TextBody = bodyText };
        builder.Attachments.Add(attachmentFileName, excelAttachment);
        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();

        try
        {
            // Timeout explícito: por defecto MailKit puede tardar mucho en fallar
            // si el host SMTP no responde; lo acotamos para dar feedback rápido al usuario.
            client.Timeout = 15000;

            await client.ConnectAsync(
                _settings.Host,
                _settings.Port,
                _settings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
                cancellationToken);

            await client.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);
            await client.SendAsync(message, cancellationToken);
        }
        catch (Exception ex) when (ex is SmtpCommandException or SmtpProtocolException or AuthenticationException)
        {
            _logger.LogError(ex, "Error SMTP al enviar correo a {ToAddress}.", toAddress);
            throw;
        }
        finally
        {
            if (client.IsConnected)
            {
                await client.DisconnectAsync(true, cancellationToken);
            }
        }
    }
}