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

    public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            page = 1;
        }

        try
        {
            var allPokemon = await _pokeApiService.GetAllPokemonAsync(cancellationToken);

            var totalCount = allPokemon.Count;

            // Paginación manual: calculamos el slice nosotros mismos con Skip/Take,
            // sin depender de ningún componente de grid con paginación automática.
            var pageItems = allPokemon
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
                TotalCount = totalCount
            };

            return View(viewModel);
        }
        catch (HttpRequestException)
        {
            _logger.LogError("No se pudo obtener el listado de Pokémon desde PokeAPI.");
            TempData["ErrorMessage"] = "No se pudo conectar con PokeAPI. Intenta de nuevo más tarde.";
            return View(new PagedPokemonViewModel());
        }
        catch (TaskCanceledException)
        {
            _logger.LogError("Timeout al obtener el listado de Pokémon desde PokeAPI.");
            TempData["ErrorMessage"] = "La solicitud a PokeAPI tardó demasiado. Intenta de nuevo.";
            return View(new PagedPokemonViewModel());
        }
    }
}