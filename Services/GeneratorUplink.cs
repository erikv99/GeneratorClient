using System.Text;
using System.Text.Json;
using GeneratorClient.Models;
using Microsoft.Extensions.Options;

namespace GeneratorClient.Services;

public class GeneratorUplink(
    ILogger<GeneratorUplink> logger,
    IOptions<GenerationSettings> settingsFromConfig,
    IHttpClientFactory httpClientFactory,
    MainDbContext dbContext)
{
    private readonly ILogger<GeneratorUplink> _logger = logger;
    private readonly MainDbContext _dbContext = dbContext;
    private readonly GenerationSettings _settingsFromConfig = settingsFromConfig.Value;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    // will be the service responsable for sending requests to the SD generator server.
    // Configure settings to set it the prompt, negative prompt, guidance scale, style strength, and inference steps.

    private GenerationSettings? _settings;

    public void Configure(GenerationSettings settings)
    {
        _settings = settings;
    }

    public async Task<GenerationResponse> SendRequestAsync()
    {
        // TODO: Add timing for request/ response.

        var generationResponse = new GenerationResponse
        {
            CreatedOn = DateTime.Now,
            Success = false
        };

        if (_settings == null)
        {
            generationResponse.ErrorMessage = "Settings must be configured before sending a request.";
            return generationResponse;
        }

        var json = JsonSerializer.Serialize(_settings);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var settings = await _saveSettingsToDbAsync(_settings);
        var request = await _saveRequestToDbAsync(settings);

        HttpResponseMessage response;

        try
        {
            response = await _httpClient.PostAsync(_settingsFromConfig.EndpointUrl, content);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error sending request to endpoint.");
            return _returnErrorResponse("Error sending request to endpoint, see logs for more information.");
        }

        if (response == null)
        {
            _logger.LogError("Request to endpoint failed, response is null.");
            return _returnErrorResponse("Request to endpoint failed, response is null.");
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Request to endpoint {_settingsFromConfig.EndpointUrl} failed with status code {response.StatusCode}");
            return _returnErrorResponse($"Request to endpoint {_settingsFromConfig.EndpointUrl} failed with status code {response.StatusCode}");
        }

        var imageBytes = await response.Content.ReadAsByteArrayAsync();

        if (imageBytes == null || imageBytes.Length == 0)
        {
            _logger.LogError("Received empty image bytes.");
            return _returnErrorResponse("Received empty image bytes.");
        }

        var image = await _saveImageToDbAsync(imageBytes);

        generationResponse.Success = true;
        generationResponse.Image = image;
        generationResponse.Request = request;

        generationResponse = await _saveResponseToDbAsync(generationResponse);
        return generationResponse;
    }

    private async Task<GenerationResponse> _saveResponseToDbAsync(GenerationResponse generationResponse)
    {
        _dbContext.Set<GenerationResponse>()
            .Add(generationResponse);

        await _dbContext.SaveChangesAsync();

        return generationResponse;
    }

    private async Task<Image> _saveImageToDbAsync(byte[] imageBytes)
    {
        var image = new Image()
        {
            Bytes = imageBytes
        };

        _dbContext.Set<Image>()
            .Add(image);

        await _dbContext.SaveChangesAsync();

        return image;
    }

    private GenerationResponse _returnErrorResponse(string errorMessage)
    {
        return new GenerationResponse
        {
            CreatedOn = DateTime.Now,
            Success = false,
            ErrorMessage = errorMessage
        };
    }

    private async Task<GenerationRequest> _saveRequestToDbAsync(GenerationSettings settings)
    {
        var request = new GenerationRequest
        {
            CreatedOn = DateTime.Now,
            Settings = settings
        };

        _dbContext.Set<GenerationRequest>()
            .Add(request);

        await _dbContext.SaveChangesAsync();
            
        return request;
    }

    private async Task<GenerationSettings> _saveSettingsToDbAsync(GenerationSettings settings)
    {
        try 
        {
            _dbContext.Set<GenerationSettings>()
                .Add(settings);

            await _dbContext.SaveChangesAsync();
            return settings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving settings to database.");
            return settings;
        }
    }
}