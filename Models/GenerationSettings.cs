using System.ComponentModel.DataAnnotations;

public class GenerationSettings
{
    [Required(ErrorMessage = "Prompt is required.")]
    [Display(Name = "Prompt")]
    public string Prompt { get; set; } = "";

    [Display(Name = "Negative Prompt")]
    public string NegativePrompt { get; set; } = "";

    [Range(0.0, 100.0, ErrorMessage = "Guidance Scale must be between 0 and 100.")]
    [Display(Name = "Guidance Scale")]
    public float GuidanceScale { get; set; } = 10.0f;

    [Range(0.0, 100.0, ErrorMessage = "Style Strength must be between 0 and 100.")]
    [Display(Name = "Style Strength")]
    public float StyleStrength { get; set; } = 0.5f;

    [Required(ErrorMessage = "Inference Steps is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Inference Steps must be a positive integer.")]
    [Display(Name = "Inference Steps")]
    public int InferenceSteps { get; set; } = 22;
}
