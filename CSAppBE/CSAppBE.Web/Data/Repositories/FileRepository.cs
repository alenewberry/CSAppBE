namespace CSAppBE.Web.Data
{
    using CSAppBE.Web.Data.Entities;

    public class FileRepository : GenericRepository<File>, IFileRepository
    {
        private readonly DataContext context;

        public FileRepository(DataContext context) : base(context)
        {
            this.context = context;
        }
    }
}
