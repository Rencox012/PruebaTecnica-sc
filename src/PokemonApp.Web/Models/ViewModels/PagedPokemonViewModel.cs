namespace PokemonApp.Web.Models.ViewModels;

/// Encapsula una "página" del listado de Pokémon junto con la metadata
/// necesaria para renderizar los controles de paginación en la vista.

public class PagedPokemonViewModel
{
    public List<PokemonListItemViewModel> Items { get; set; } = new();
    public PokemonFilterViewModel Filters { get; set; } = new();

    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}