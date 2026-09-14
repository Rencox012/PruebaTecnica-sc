namespace PokemonApp.Web.Models.ViewModels;

public class SendEmailViewModel
{
    public string ToAddress { get; set; } = string.Empty;

    ///Si viene con valor, es envío individual de un solo Pokémon (por Id).
    /// Si es null, es envío general de la página actual completa.
    public int? PokemonId { get; set; }

    public string? Name { get; set; }
    public string? Species { get; set; }
    public int Page { get; set; } = 1;
}