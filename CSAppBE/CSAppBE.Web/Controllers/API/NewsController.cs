namespace CSAppBE.Web.Controllers.API
{
    using CSAppBE.Web.Data.Entities;
    using CSAppBE.Web.Data.Repositories;
    using CSAppBE.Web.Helpers;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NewsController : Controller
    {
        private readonly INewsRepository newsRepo;
        private readonly IUserHelper userHelper;

        public NewsController(IUserHelper userHelper, INewsRepository newsRepo)
        {
            this.userHelper = userHelper;
            this.newsRepo = newsRepo;
        }

        [HttpGet]
        public IActionResult GetNews()
        {
            return Ok(this.newsRepo.GetAll());
        }

        [HttpPost]
        public async Task<IActionResult> PostNews([FromBody] CSAppFE.Common.Models.News news)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            var entityNews = new News
            {
                Title = news.Title,
                Description = news.Description,
                Link = news.Link,
                PublicatedDate = news.PublicatedDate,
                Type = news.Type
            };

            var newNews = await this.newsRepo.CreateAsync(entityNews);
            return Ok(newNews);
        }
    }
}
