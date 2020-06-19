namespace CSAppBE.Web.Data
{
    using Entities;
    using System.Linq;

    public interface IClientRepository : IGenericRepository<Client>
    {
        IQueryable GetAllWithUsers();

        IQueryable GetAllByUser(string email);
    }

}
