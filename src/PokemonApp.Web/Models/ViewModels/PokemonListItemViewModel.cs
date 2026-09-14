namespace PokemonApp.Web.Models.ViewModels;


/// Representa una fila del grid de Pokémon en la vista.

public class PokemonListItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}