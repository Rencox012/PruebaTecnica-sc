using Microsoft.AspNetCore.Mvc;
using PokemonApp.Web.Models.ViewModels;
using PokemonApp.Web.Services;


namespace PokemonApp.Web.Controllers;

public class PokemonController : Controller
{
    private const string SpriteBaseUrl =
        "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/";

    private readonly IPokeApiService _pokeApiService;
    private readonly ILogger<PokemonController> _logger;

    public PokemonController(IPokeApiService pokeApiService, ILogger<PokemonController> logger)
    {
        _pokeApiService = pokeApiService;
        _logger = logger;
    }

    private const int DefaultPageSize = 20;

    public async Task<IActionResult> Index(
    string? name,
    string? species,
    int page = 1,
    CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            page = 1;
        }

        try
        {
            var allPokemon = await _pokeApiService.GetAllPokemonAsync(cancellationToken);
            var allSpecies = await _pokeApiService.GetAllSpeciesAsync(cancellationToken);

            // Filtrado en memoria sobre el catálogo completo (ya cacheado)
            var filtered = allPokemon.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                filtered = filtered.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(species))
            {
                // Supuesto: el nombre de especie coincide con el nombre base del Pokémon
                // (ver nota en PokeApiService.GetAllSpeciesAsync).
                filtered = filtered.Where(p => p.Name.Equals(species, StringComparison.OrdinalIgnoreCase));
            }

            var filteredList = filtered.ToList();
            var totalCount = filteredList.Count;

            var pageItems = filteredList
                .Skip((page - 1) * DefaultPageSize)
                .Take(DefaultPageSize)
                .Select(p => new PokemonListItemViewModel
                {
                    Id = p.GetId(),
                    Name = p.Name,
                    ImageUrl = $"{SpriteBaseUrl}{p.GetId()}.png"
                })
                .ToList();

            var viewModel = new PagedPokemonViewModel
            {
                Items = pageItems,
                CurrentPage = page,
                PageSize = DefaultPageSize,
                TotalCount = totalCount,
                Filters = new PokemonFilterViewModel
                {
                    NameFilter = name,
                    SpeciesFilter = species,
                    AvailableSpecies = allSpecies.Select(s => s.Name).OrderBy(n => n).ToList()
                }
            };

            return View(viewModel);
        }
        catch (HttpRequestException)
        {
            _logger.LogError("No se pudo obtener datos desde PokeAPI.");
            TempData["ErrorMessage"] = "No se pudo conectar con PokeAPI. Intenta de nuevo más tarde.";
            return View(new PagedPokemonViewModel());
        }
        catch (TaskCanceledException)
        {
            _logger.LogError("Timeout al consultar PokeAPI.");
            TempData["ErrorMessage"] = "La solicitud a PokeAPI tardó demasiado. Intenta de nuevo.";
            return View(new PagedPokemonViewModel());
        }
    }
}