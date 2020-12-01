namespace CSAppBE.Web.Controllers
{
    using System.Diagnostics;
    using Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using CSAppBE.Web.Data.Repositories;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INewsRepository newsRepo;

        public HomeController(ILogger<HomeController> logger, INewsRepository newsRepo)
        {
            _logger = logger;
            this.newsRepo = newsRepo;
        }

        public IActionResult Index()
        {
            @ViewBag.News = this.newsRepo.GetLastNews().Description;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("error/404")]
        public IActionResult Error404()
        {
            return View();
        }

        [Route("error/100")]
        public IActionResult Error100()
        {
            return View();
        }
    }
}
