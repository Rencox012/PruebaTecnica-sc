namespace PokemonApp.Web.Models.Dtos;

public class PokemonSpeciesListResponseDto
{
    public int Count { get; set; }
    public List<PokemonSpeciesListItemDto> Results { get; set; } = new();
}