using ClosedXML.Excel;
using PokemonApp.Web.Models.ViewModels;

namespace PokemonApp.Web.Services;

public class ExcelExportService : IExcelExportService
{
    public byte[] ExportToExcel(List<PokemonListItemViewModel> pokemon)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Pokémon");

        worksheet.Cell(1, 1).Value = "ID";
        worksheet.Cell(1, 2).Value = "Nombre";
        worksheet.Cell(1, 3).Value = "URL de imagen";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;

        for (var i = 0; i < pokemon.Count; i++)
        {
            var row = i + 2;
            worksheet.Cell(row, 1).Value = pokemon[i].Id;
            worksheet.Cell(row, 2).Value = pokemon[i].Name;
            worksheet.Cell(row, 3).Value = pokemon[i].ImageUrl;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}