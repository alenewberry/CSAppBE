namespace CSAppBE.Web.Data.Repositories
{
    using CSAppBE.Web.Data.Entities;
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


    }
}
