namespace CSAppBE.Web.Controllers
{
    using CSAppBE.Web.Data.Entities;
    using CSAppBE.Web.Data.Repositories;
    using CSAppBE.Web.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    public class NewsController : Controller
    {
        private readonly INewsRepository newsRepo;

        public NewsController(INewsRepository newsRepo)
        {
            this.newsRepo = newsRepo;
        }

        public IActionResult Index()
        {
            return View(this.newsRepo.GetAll());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(News news)
        {
            if (ModelState.IsValid)
            {
                await this.newsRepo.CreateAsync(news);
                return RedirectToAction(nameof(Index));
            }

            return View(news);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("NewsNotFound");
            }
            var news = await this.newsRepo.GetByIdAsync(id.Value);
            if (news == null)
            {
                return new NotFoundViewResult("NewsNotFound");
            }
            return View(news);
        }
    }
}
