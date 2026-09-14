using PokemonApp.Web.Models.Dtos;

namespace PokemonApp.Web.Services;

public interface IPokeApiService
{
    /// Devuelve el listado completo de Pokémon (nombre + url), cacheado en memoria.
    /// La paginación y el filtrado sobre este listado se resuelven en el controlador/servicio
    /// de más alto nivel, no aquí — este método solo abstrae la obtención cruda + cache.

    Task<List<PokemonListItemDto>> GetAllPokemonAsync(CancellationToken cancellationToken = default);
}