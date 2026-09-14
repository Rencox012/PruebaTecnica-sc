using System.Text.Json.Serialization;

namespace PokemonApp.Web.Models.Dtos;

public class PokemonSpeciesDetailDto
{
    [JsonPropertyName("flavor_text_entries")]
    public List<FlavorTextEntryDto> FlavorTextEntries { get; set; } = new();

    public PokemonHabitatDto? Habitat { get; set; }
}

public class FlavorTextEntryDto
{
    [JsonPropertyName("flavor_text")]
    public string FlavorText { get; set; } = string.Empty;

    public PokemonLanguageDto Language { get; set; } = new();
}

public class PokemonLanguageDto
{
    public string Name { get; set; } = string.Empty;
}

public class PokemonHabitatDto
{
    public string Name { get; set; } = string.Empty;
}