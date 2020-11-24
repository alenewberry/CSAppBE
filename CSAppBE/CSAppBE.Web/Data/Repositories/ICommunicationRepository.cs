namespace CSAppBE.Web.Data.Repositories
{
    using CSAppBE.Web.Data.Entities;
    using System.Linq;

    public interface ICommunicationRepository : IGenericRepository<Communication>
    {
        IQueryable<Communication> GetByClientId(string cuit);
        Communication GetByCommunicationId(long commId);
    }
}
