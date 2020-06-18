namespace CSAppBE.Web.Data
{
	using System.Linq;
	using Entities;
    using Microsoft.EntityFrameworkCore;

    public class ClientRepository : GenericRepository<Client>, IClientRepository
	{
		private readonly DataContext context;

		public ClientRepository(DataContext context) : base(context)
		{
			this.context = context;
		}

		public IQueryable GetAllWithUsers()
		{
			return this.context.Clients.Include(p => p.User).OrderBy(p => p.Name);
		}
	}

}
