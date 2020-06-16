namespace CSAppBE.Web.Data
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Entities;

	public class Repository : IRepository
	{
		private readonly DataContext context;

		public Repository(DataContext context)
		{
			this.context = context;
		}

		public IEnumerable<Client> GetClients()
		{
			return this.context.Clients.OrderBy(p => p.Name);
		}

		public Client GetClient(int id)
		{
			return this.context.Clients.Find(id);
		}

		public void AddClient(Client client)
		{
			this.context.Clients.Add(client);
		}

		public void UpdateClient(Client client)
		{
			this.context.Update(client);
		}

		public void RemoveClient(Client client)
		{
			this.context.Clients.Remove(client);
		}

		public async Task<bool> SaveAllAsync()
		{
			return await this.context.SaveChangesAsync() > 0;
		}

		public bool ClientExists(int id)
		{
			return this.context.Clients.Any(p => p.Id == id);
		}
	}

}
