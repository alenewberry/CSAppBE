namespace CSAppBE.Web.Data.Repositories
{
    using CSAppBE.Web.Data.Entities;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;

    public class CommunicationRepository : GenericRepository<Communication>, ICommunicationRepository
    {
        private readonly DataContext context;

        public CommunicationRepository(DataContext context) : base(context)
        {
            this.context = context;
        }

        public Communication GetByCommunicationId(long commId)
        {
            return this.context.Communications.Where(c => c.CommunicationId == commId).FirstOrDefault();
        }

        public IQueryable<Communication> GetByClientId(string cuit)
        {
            return this.context.Communications.Include(c => c.Client).Where(c => c.Cuit == cuit).OrderByDescending(c => c.PublishedDate);
        }


    }
}
