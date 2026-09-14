namespace PokemonApp.Web.Models.Dtos;

public class PokemonListItemDto
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    public int GetId(){
        var trimmer = Url.TrimEnd('/');
        var lastSegment = trimmer.Substring(trimmer.LastIndexOf('/') + 1);
        return int.TryParse(lastSegment, out var id) ? id : 0;    }
}