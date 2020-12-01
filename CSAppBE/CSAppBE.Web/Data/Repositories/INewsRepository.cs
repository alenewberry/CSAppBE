namespace CSAppBE.Web.Data.Repositories
{
    using CSAppBE.Web.Data.Entities;


    public interface INewsRepository : IGenericRepository<News>
    {
        public News GetLastNews();
    }
}
