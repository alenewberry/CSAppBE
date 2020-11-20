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

		public Client GetByCUIT(string CUIT)
        {
			return this.context.Clients.Include(c => c.User).Where(c => c.CUIT == CUIT).FirstOrDefault();
		}

		public IQueryable GetAllWithUsers()
		{
			return this.context.Clients.Include(p => p.User).OrderBy(p => p.Name);
		}

		public IQueryable GetAllByUser(string email)
		{
			return this.context.Clients.Include(p => p.User).Where(p => p.User.Name == email).OrderBy(p => p.Name);
		}
	}
}
