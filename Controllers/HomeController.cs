using GeneratorClient.Models;
using GeneratorClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace GeneratorClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GeneratorUplink _generatorUplink;
        private readonly GenerationSettings _settingsFromConfig;

        public HomeController(
            ILogger<HomeController> logger,
            GeneratorUplink generatorUplink,
            IOptions<GenerationSettings> settingsFromConfig)
        {
            _logger = logger;
            _generatorUplink = generatorUplink;
            _settingsFromConfig = settingsFromConfig.Value;
        }

        public IActionResult Index()
        {
            return View(new HomeVm() 
            { 
                GenerationSettings = _settingsFromConfig
            });
        }

        [HttpGet]
        public async Task<IActionResult> Configurations()
        {
            return PartialView("~/Views/Shared/_ConfigModePartial.cshtml");
        }

        // Todo, naming is a bit weird, fix.
        [HttpGet]
        public async Task<IActionResult> OutputOverview() 
        {
            return PartialView("~/Views/Shared/_OutputOverviewPartial.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> OutputCurrent(string currentImageUrl = "https://placehold.co/600x400") 
        {
            return PartialView("~/Views/Shared/_OutputCurrentPartial.cshtml", currentImageUrl);
        }

        [HttpPost]
        public async Task<IActionResult> QuickRequest(GenerationSettings model)
        {
            var viewModel = new HomeVm
            {
                GenerationSettings = model,
            };

            if (!ModelState.IsValid)
            {
                return PartialView("~/Views/Shared/_QuickModePartial.cshtml", model);
            }

            _generatorUplink.ConfigureSettings(model);

            var (_isGenerationSuccessful, _pathToImg) = await _generatorUplink.SendRequestAsync();
            
            if (!_isGenerationSuccessful)
            {
                ModelState.AddModelError(string.Empty, "Error generating image.");
                _logger.LogError("Error generating image.");
                return PartialView("~/Views/Shared/_QuickModePartial.cshtml", model);
            }

            _logger.LogInformation("Image generated successfully at path {Path}", _pathToImg);
            viewModel.ImageUrl = _pathToImg;

            // TODO save location and used setting to a log in the db.

            return PartialView("~/Views/Shared/_QuickModePartial.cshtml", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
