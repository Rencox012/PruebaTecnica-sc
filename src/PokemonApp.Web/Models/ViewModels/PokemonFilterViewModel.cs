namespace PokemonApp.Web.Models.ViewModels;

/// Filtros aplicables sobre el catálogo de Pokémon. Se usa tanto para
/// leer los parámetros de la query string como para poblar los controles
/// de filtro en la vista (manteniendo el valor seleccionado tras el postback).

public class PokemonFilterViewModel
{
    public string? NameFilter { get; set; }
    public string? SpeciesFilter { get; set; }

    public List<string> AvailableSpecies { get; set; } = new();
}