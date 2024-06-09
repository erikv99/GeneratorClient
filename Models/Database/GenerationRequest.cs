using GeneratorClient.Models;

public class GenerationRequest
{
    public int? Id { get; set; }
    public DateTime CreatedOn { get; set; }

    public int SettingsId { get; set; }
    public GenerationSettings Settings { get; set; } = null!;
}