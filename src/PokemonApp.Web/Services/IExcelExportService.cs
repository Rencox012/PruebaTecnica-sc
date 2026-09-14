using PokemonApp.Web.Models.ViewModels;

namespace PokemonApp.Web.Services;

public interface IExcelExportService
{
    /// Genera un archivo .xlsx en memoria a partir de la lista de Pokémon dada.
    byte[] ExportToExcel(List<PokemonListItemViewModel> pokemon);
}