namespace PokemonApp.Web.Models.Dtos;

/// <summary>
/// Representa un elemento del listado de especies de PokeAPI (/pokemon-species).
/// Se usa únicamente para poblar el dropdown de filtro 

public class PokemonSpeciesListItemDto
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}