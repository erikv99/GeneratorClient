namespace GeneratorClient.Models;

public class HomeVm
{
    public GenerationSettings GenerationSettings { get; set; } = new GenerationSettings();

    public string? ImageUrl { get; set; }
}
