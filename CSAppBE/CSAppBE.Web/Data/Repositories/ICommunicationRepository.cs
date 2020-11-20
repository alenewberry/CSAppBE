namespace CSAppBE.Web.Data.Repositories
{
    using CSAppBE.Web.Data.Entities;
    using System.Linq;

    public interface ICommunicationRepository : IGenericRepository<Communication>
    {
        Communication GetByCommunicationId(long commId);
    }
}
