namespace CSAppBE.Web.Controllers.API
{
    using CSAppBE.Web.Data.Entities;
    using CSAppBE.Web.Helpers;
    using Data;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ClientsController : Controller
    {
        private readonly IClientRepository clientRepo;
        private readonly IUserHelper userHelper;

        public ClientsController(IClientRepository clientRepo, IUserHelper userHelper)
        {
            this.clientRepo = clientRepo;
            this.userHelper = userHelper;
        }

        [HttpGet]
        public IActionResult GetClients()
        {
            return Ok(this.clientRepo.GetAllWithUsers());
        }

		[HttpPost]
		public async Task<IActionResult> PostClient([FromBody] CSAppFE.Common.Models.Client client)
		{
			if (!ModelState.IsValid)
			{
				return this.BadRequest(ModelState);
			}

			var user = await this.userHelper.GetUserByEmailAsync(client.User.Email);
			if (user == null)
			{
				return this.BadRequest("Invalid user");
			}

			//TODO: Upload images
			var entityClient = new Client
			{
				CUIT = client.Cuit,
				Name = client.Name,
				Email = client.Email,
				Phone = client.Phone,
				User = user
			};

			var newClient = await this.clientRepo.CreateAsync(entityClient);
			return Ok(newClient);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> PutClient([FromRoute] int id, [FromBody] CSAppFE.Common.Models.Client client)
		{
			if (!ModelState.IsValid)
			{
				return this.BadRequest(ModelState);
			}

			if (id != client.Id)
			{
				return BadRequest();
			}

			var oldClient = await this.clientRepo.GetByIdAsync(id);
			if (oldClient == null)
			{
				return this.BadRequest("Client Id don't exists.");
			}

			//TODO: Upload images
			oldClient.CUIT = client.Cuit;
			oldClient.Name = client.Name;
			oldClient.Email = client.Email;
			oldClient.Phone = client.Phone;

			var updatedClient = await this.clientRepo.UpdateAsync(oldClient);
			return Ok(updatedClient);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteClient([FromRoute] int id)
		{
			if (!ModelState.IsValid)
			{
				return this.BadRequest(ModelState);
			}

			var client = await this.clientRepo.GetByIdAsync(id);
			if (client == null)
			{
				return this.NotFound();
			}

			await this.clientRepo.DeleteAsync(client);
			return Ok(client);
		}
	}
}
