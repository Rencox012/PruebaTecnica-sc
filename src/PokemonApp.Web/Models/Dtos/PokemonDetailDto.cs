using System.Text.Json.Serialization;

namespace PokemonApp.Web.Models.Dtos;

public class PokemonDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Height { get; set; }
    public int Weight { get; set; }

    [JsonPropertyName("types")]
    public List<PokemonTypeSlotDto> Types { get; set; } = new();
}

public class PokemonTypeSlotDto
{
    public PokemonTypeInfoDto Type { get; set; } = new();
}

public class PokemonTypeInfoDto
{
    public string Name { get; set; } = string.Empty;
}