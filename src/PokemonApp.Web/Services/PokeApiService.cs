using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using PokemonApp.Web.Models.Dtos;

namespace PokemonApp.Web.Services;

public class PokeApiService : IPokeApiService
{
    private const string AllPokemonCacheKey = "poke:all-pokemon";

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
}