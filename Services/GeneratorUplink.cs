public class GeneratorUplink
{
    private readonly ILogger<GeneratorUplink> _logger;

    // will be the service responsable for sending requests to the SD generator server.
    // Configure settings to set it the prompt, negative prompt, guidance scale, style strength, and inference steps.

    private GenerationSettings? _settings;

    public GeneratorUplink(
        ILogger<GeneratorUplink> logger)
    {
        _logger = logger;
    }

    public void ConfigureSettings(GenerationSettings settings)
    {
        _settings = settings;
    }

    public async Task<bool> SendRequestAsync()
    {
        if (_settings == null)
        {
            // TODO, friendler handling of empty settings.
            throw new InvalidOperationException("Settings must be configured before sending a request.");
        }

        // Use the current settings to make a post request to the server.

        await Task.CompletedTask;
        return true;
    }
}