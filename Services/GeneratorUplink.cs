using System.Text;
using System.Text.Json;
using GeneratorClient.Models;
using Microsoft.Extensions.Options;

namespace GeneratorClient.Services;

public class GeneratorUplink(
    ILogger<GeneratorUplink> logger,
    IOptions<GenerationSettings> settingsFromConfig,
    IHttpClientFactory httpClientFactory)
{
    private readonly ILogger<GeneratorUplink> _logger = logger;
    private readonly GenerationSettings _settingsFromConfig = settingsFromConfig.Value;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    // will be the service responsable for sending requests to the SD generator server.
    // Configure settings to set it the prompt, negative prompt, guidance scale, style strength, and inference steps.

    private GenerationSettings? _settings;

    public void Configure(GenerationSettings settings)
    {
        _settings = settings;
    }

    public async Task<(bool Success, string? ImageUrl)> SendRequestAsync()
    {
        if (_settings == null)
        {
            // TODO, friendler handling of empty settings.
            throw new InvalidOperationException("Settings must be configured before sending a request.");
        }

        var json = JsonSerializer.Serialize(_settings);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response;

        // TODO: Refactor if satisfied with the current implementation.

        try
        {
            response = await _httpClient.PostAsync(_settingsFromConfig.EndpointUrl, content);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error sending request to endpoint {Endpoint}", _settingsFromConfig.EndpointUrl);
            return (false, null);
        }

        if (response == null) {
            _logger.LogError("Request to endpoint {Endpoint} failed, response is null.", _settingsFromConfig.EndpointUrl);
            return (false, null);
        }
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Request to endpoint {Endpoint} failed with status code {StatusCode}", 
                _settingsFromConfig.EndpointUrl, response.StatusCode);
            return (false, null);
        }

        var imageBytes = await response.Content.ReadAsByteArrayAsync();
        var outputFolderPath = "output";

        _ensureFolderExists(outputFolderPath);

        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var imagePath = Path.Combine(outputFolderPath, $"generation-{timestamp}.png");

        await File.WriteAllBytesAsync(imagePath, imageBytes);
        _logger.LogInformation("Image saved to {ImagePath}", imagePath);

        return (true, imagePath);
    }

    private void _ensureFolderExists(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }

}