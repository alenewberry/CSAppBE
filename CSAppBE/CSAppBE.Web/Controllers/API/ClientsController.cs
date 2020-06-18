namespace CSAppBE.Web.Controllers.API
{
    using Data;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
