using System.Text.Json.Serialization;

namespace PokemonApp.Web.Models.Dtos;

/// Respuesta cruda del endpoint GET /pokemon de PokeAPI.

public class PokemonListResponseDto
{
    public int Count { get; set; }

    [JsonPropertyName("next")]
    public string? Next { get; set; }

    [JsonPropertyName("previous")]
    public string? Previous { get; set; }

    public List<PokemonListItemDto> Results { get; set; } = new();
}