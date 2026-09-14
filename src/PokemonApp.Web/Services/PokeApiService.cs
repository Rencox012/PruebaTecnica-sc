using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using PokemonApp.Web.Models.Dtos;

namespace PokemonApp.Web.Services;

public class PokeApiService : IPokeApiService
{
    private const string AllPokemonCacheKey = "poke:all-pokemon";
    private const string AllSpeciesCacheKey = "poke:all-species";

    // PokeAPI tiene ~1300 Pokémon actualmente; pedimos un límite alto
    // para traer el catálogo completo en una sola llamada y cachearlo.
    private const int FullCatalogLimit = 2000;

    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<PokeApiService> _logger;

    public PokeApiService(HttpClient httpClient, IMemoryCache cache, ILogger<PokeApiService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<PokemonListItemDto>> GetAllPokemonAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(AllPokemonCacheKey, out List<PokemonListItemDto>? cached) && cached is not null)
        {
            return cached;
        }

        try
        {
            var response = await _httpClient.GetFromJsonAsync<PokemonListResponseDto>(
                $"pokemon?limit={FullCatalogLimit}&offset=0",
                cancellationToken);

            var results = response?.Results ?? new List<PokemonListItemDto>();

            _cache.Set(AllPokemonCacheKey, results, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6)
            });

            return results;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de red al consultar el listado de PokeAPI.");
            throw;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            // TaskCanceledException sin cancelación explícita del caller = timeout del HttpClient.
            _logger.LogError(ex, "Timeout al consultar el listado de PokeAPI.");
            throw;
        }
    }

    public async Task<List<PokemonSpeciesListItemDto>> GetAllSpeciesAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(AllSpeciesCacheKey, out List<PokemonSpeciesListItemDto>? cached) && cached is not null)
        {
            return cached;
        }

        try
        {
            var response = await _httpClient.GetFromJsonAsync<PokemonSpeciesListResponseDto>(
                $"pokemon-species?limit={FullCatalogLimit}&offset=0",
                cancellationToken);

            var results = response?.Results ?? new List<PokemonSpeciesListItemDto>();

            _cache.Set(AllSpeciesCacheKey, results, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6)
            });

            return results;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de red al consultar el listado de especies de PokeAPI.");
            throw;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout al consultar el listado de especies de PokeAPI.");
            throw;
        }
    }

    public async Task<PokemonDetailDto?> GetPokemonDetailAsync(int id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"poke:detail:{id}";

        if (_cache.TryGetValue(cacheKey, out PokemonDetailDto? cached) && cached is not null)
        {
            return cached;
        }

        try
        {
            var detail = await _httpClient.GetFromJsonAsync<PokemonDetailDto>($"pokemon/{id}", cancellationToken);

            if (detail is not null)
            {
                _cache.Set(cacheKey, detail, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6)
                });
            }

            return detail;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de red al consultar el detalle del Pokémon {Id}.", id);
            throw;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout al consultar el detalle del Pokémon {Id}.", id);
            throw;
        }
    }

    public async Task<PokemonSpeciesDetailDto?> GetSpeciesDetailAsync(int id, CancellationToken cancellationToken = default)
    {
        // Cache independiente del cache de detalle de Pokémon: la especie se pide
        // por separado 
        var cacheKey = $"poke:species-detail:{id}";

        if (_cache.TryGetValue(cacheKey, out PokemonSpeciesDetailDto? cached) && cached is not null)
        {
            return cached;
        }

        try
        {
            var detail = await _httpClient.GetFromJsonAsync<PokemonSpeciesDetailDto>(
                $"pokemon-species/{id}", cancellationToken);

            if (detail is not null)
            {
                _cache.Set(cacheKey, detail, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6)
                });
            }

            return detail;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de red al consultar la especie del Pokémon {Id}.", id);
            throw;
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout al consultar la especie del Pokémon {Id}.", id);
            throw;
        }
    }

}