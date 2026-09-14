using Microsoft.AspNetCore.Mvc;
using PokemonApp.Web.Models.ViewModels;
using PokemonApp.Web.Services;
using PokemonApp.Web.Models.Dtos;


namespace PokemonApp.Web.Controllers;

public class PokemonController : Controller
{
    private const string SpriteBaseUrl =
        "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/";

    private readonly IPokeApiService _pokeApiService;
    private readonly ILogger<PokemonController> _logger;
    private readonly IExcelExportService _excelExportService;
    private readonly IEmailService _emailService;
    public PokemonController(
    IPokeApiService pokeApiService,
    IExcelExportService excelExportService,
    ILogger<PokemonController> logger,
    IEmailService emailService)
    {
        _pokeApiService = pokeApiService;
        _excelExportService = excelExportService;
        _logger = logger;
        _emailService = emailService;
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
            var filteredList = await GetFilteredPokemonAsync(name, species, cancellationToken);
            var allSpecies = await _pokeApiService.GetAllSpeciesAsync(cancellationToken);


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

    public async Task<IActionResult> Detail(int id, CancellationToken cancellationToken)
    {
        try
        {
            var detail = await _pokeApiService.GetPokemonDetailAsync(id, cancellationToken);

            if (detail is null)
            {
                return NotFound();
            }

            var species = await _pokeApiService.GetSpeciesDetailAsync(id, cancellationToken);

            var flavorText = species?.FlavorTextEntries
                .FirstOrDefault(f => f.Language.Name == "es")?.FlavorText
                ?? species?.FlavorTextEntries.FirstOrDefault(f => f.Language.Name == "en")?.FlavorText;

            var viewModel = new PokemonDetailViewModel
            {
                Id = detail.Id,
                Name = detail.Name,
                ImageUrl = $"{SpriteBaseUrl}{detail.Id}.png",
                HeightDecimeters = detail.Height,
                WeightHectograms = detail.Weight,
                Types = detail.Types.Select(t => t.Type.Name).ToList(),
                FlavorText = flavorText?.Replace("\n", " ").Replace("\f", " "),
                Habitat = species?.Habitat?.Name
            };

            return PartialView("_DetailModalPartial", viewModel);
        }
        catch (HttpRequestException)
        {
            _logger.LogError("No se pudo obtener el detalle del Pokémon {Id}.", id);
            return StatusCode(502, "No se pudo conectar con PokeAPI.");
        }
        catch (TaskCanceledException)
        {
            _logger.LogError("Timeout al obtener el detalle del Pokémon {Id}.", id);
            return StatusCode(504, "La solicitud a PokeAPI tardó demasiado.");
        }
    }

    private async Task<List<PokemonListItemDto>> GetFilteredPokemonAsync(
    string? name, string? species, CancellationToken cancellationToken)
    {
        var allPokemon = await _pokeApiService.GetAllPokemonAsync(cancellationToken);
        var filtered = allPokemon.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            filtered = filtered.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(species))
        {
            filtered = filtered.Where(p => p.Name.Equals(species, StringComparison.OrdinalIgnoreCase));
        }

        return filtered.ToList();
    }

    public async Task<IActionResult> ExportExcel(
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
        var filteredList = await GetFilteredPokemonAsync(name, species, cancellationToken);

        // Exportamos SOLO la página actual.
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

        var fileBytes = _excelExportService.ExportToExcel(pageItems);

        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"pokemon-pagina-{page}.xlsx");
    }
    catch (HttpRequestException)
    {
        _logger.LogError("No se pudo obtener datos de PokeAPI para exportar.");
        TempData["ErrorMessage"] = "No se pudo generar el Excel: error al conectar con PokeAPI.";
        return RedirectToAction(nameof(Index), new { name, species, page });
    }
}


    [HttpGet]
    public IActionResult SendEmailForm(int? pokemonId, string? name, string? species, int page = 1)
    {
        var viewModel = new SendEmailViewModel
        {
            PokemonId = pokemonId,
            Name = name,
            Species = species,
            Page = page
        };

        return PartialView("_SendEmailModalPartial", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendEmail(SendEmailViewModel model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.ToAddress))
        {
            TempData["ErrorMessage"] = "Debes indicar un correo destino.";
            return RedirectToAction(nameof(Index), new { name = model.Name, species = model.Species, page = model.Page });
        }

        try
        {
            byte[] excelBytes;
            string subject;
            string fileName;

            if (model.PokemonId.HasValue)
            {
                // Envío individual: solo ese Pokémon.
                var detail = await _pokeApiService.GetPokemonDetailAsync(model.PokemonId.Value, cancellationToken);

                if (detail is null)
                {
                    TempData["ErrorMessage"] = "No se encontró el Pokémon a enviar.";
                    return RedirectToAction(nameof(Index), new { name = model.Name, species = model.Species, page = model.Page });
                }

                var singleItem = new List<PokemonListItemViewModel>
                {
                    new() { Id = detail.Id, Name = detail.Name, ImageUrl = $"{SpriteBaseUrl}{detail.Id}.png" }
                };

                excelBytes = _excelExportService.ExportToExcel(singleItem);
                subject = $"Reporte Pokémon: {detail.Name}";
                fileName = $"pokemon-{detail.Name}.xlsx";
            }
            else
            {
                // Envío general: la página actual completa.
                var filteredList = await GetFilteredPokemonAsync(model.Name, model.Species, cancellationToken);

                var pageItems = filteredList
                    .Skip((model.Page - 1) * DefaultPageSize)
                    .Take(DefaultPageSize)
                    .Select(p => new PokemonListItemViewModel
                    {
                        Id = p.GetId(),
                        Name = p.Name,
                        ImageUrl = $"{SpriteBaseUrl}{p.GetId()}.png"
                    })
                    .ToList();

                excelBytes = _excelExportService.ExportToExcel(pageItems);
                subject = $"Reporte Pokémon: página {model.Page}";
                fileName = $"pokemon-pagina-{model.Page}.xlsx";
            }

            await _emailService.SendPokemonReportAsync(
                model.ToAddress,
                subject,
                "Adjunto el reporte de Pokémon solicitado.",
                excelBytes,
                fileName,
                cancellationToken);

            TempData["SuccessMessage"] = $"Correo enviado a {model.ToAddress}.";
        }
        catch (Exception)
        {
            _logger.LogError("Error al enviar el correo a {ToAddress}.", model.ToAddress);
            TempData["ErrorMessage"] = "No se pudo enviar el correo. Verifica la configuración SMTP.";
        }

        return RedirectToAction(nameof(Index), new { name = model.Name, species = model.Species, page = model.Page });
    }
}