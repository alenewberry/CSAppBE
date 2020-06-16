namespace CSAppBE.Web.Data
{
	using Entities;

	public class ClientRepository : GenericRepository<Client>, IClientRepository
	{
		public ClientRepository(DataContext context) : base(context)
		{
		}
	}

}
