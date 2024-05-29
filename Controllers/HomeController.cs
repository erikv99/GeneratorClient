using GeneratorClient.Models;
using GeneratorClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace GeneratorClient.Controllers
{
    public class HomeController(
        ILogger<HomeController> logger,
        GeneratorUplink generatorUplink,
        IOptions<GenerationSettings> settingsFromConfig) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly GeneratorUplink _generatorUplink = generatorUplink;
        private readonly GenerationSettings _settingsFromConfig = settingsFromConfig.Value;

        public IActionResult Index()
        {
            return View(new HomeVm() 
            { 
                GenerationSettings = _settingsFromConfig
            });
        }

        [HttpGet]
        public IActionResult ConfigurationsMode()
        {
            return PartialView("~/Views/Shared/_ConfigModePartial.cshtml");
        }

        // Todo, naming is a bit weird, fix.
        [HttpGet]
        public IActionResult OutputOverview()
        {
            return PartialView("~/Views/Shared/_OutputOverviewPartial.cshtml");
        }

        [HttpGet]
        public IActionResult OutputCurrent(string currentImageUrl)
        {
            return PartialView("~/Views/Shared/_OutputCurrentPartial.cshtml", currentImageUrl);
        }

        [HttpGet]
        public IActionResult QuickMode()
        {
            return PartialView("~/Views/Shared/_QuickModePartial.cshtml", _settingsFromConfig);
        }

        [HttpPost]
        public async Task<IActionResult> QuickRequest(GenerationSettings model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new 
                { 
                    error = "Invalid model state." 
                });
            }

            _generatorUplink.Configure(model);

            var (_isGenerationSuccessful, _pathToImg) = await _generatorUplink.SendRequestAsync();

            if (!_isGenerationSuccessful)
            {
                return BadRequest(new
                {
                    error = "Image generation did not indicate success."
                });
            }

            return Ok(new 
            { 
                path = _pathToImg ?? ""
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
