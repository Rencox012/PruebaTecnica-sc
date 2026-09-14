namespace PokemonApp.Web.Services;

public interface IEmailService
{

    /// Envía un correo con el archivo Excel adjunto.
    /// Se usa tanto para envío individual (un Pokémon) como general (la página completa)

    Task SendPokemonReportAsync(string toAddress, string subject, string bodyText, byte[] excelAttachment, string attachmentFileName, CancellationToken cancellationToken = default);
}