namespace PokemonApp.Web.Models.ViewModels;

public class PokemonDetailViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int HeightDecimeters { get; set; }
    public int WeightHectograms { get; set; }
    public List<string> Types { get; set; } = new();
    public string? FlavorText { get; set; }
    public string? Habitat { get; set; }
}