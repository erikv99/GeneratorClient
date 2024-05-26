using GeneratorClient.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GeneratorClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GeneratorUplink _generatorUplink;

        public HomeController(
            ILogger<HomeController> logger,
            GeneratorUplink generatorUplink)
        {
            _logger = logger;
            _generatorUplink = generatorUplink;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> QuickRequest(GenerationSettings model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            _generatorUplink.ConfigureSettings(model);
            await _generatorUplink.SendRequestAsync();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
