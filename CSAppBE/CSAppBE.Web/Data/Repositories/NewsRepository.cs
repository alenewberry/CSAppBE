namespace CSAppBE.Web.Data.Repositories
{
    using CSAppBE.Web.Data.Entities;
    using System.Linq;

    public class NewsRepository : GenericRepository<News>, INewsRepository
    {
        private readonly DataContext context;
        public NewsRepository(DataContext context) : base(context)
        {
            this.context = context;
        }

        public News GetLastNews()
        {
            return this.context.News.OrderByDescending(n => n.Id).FirstOrDefault();
        }

    }
}
