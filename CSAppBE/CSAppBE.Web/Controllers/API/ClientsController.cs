namespace CSAppBE.Web.Controllers.API
{
    using Data;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[Controller]")]
    public class ClientsController : Controller
    {
        private readonly IClientRepository clientRepo;

        public ClientsController(IClientRepository clientRepo)
        {
            this.clientRepo = clientRepo;
        }

        [HttpGet]
        public IActionResult GetClients()
        {
            return Ok(this.clientRepo.GetAllWithUsers());
        }
    }
}
