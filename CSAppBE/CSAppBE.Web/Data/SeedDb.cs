namespace CSAppBE.Web.Data
{
    using Entities;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class SeedDb
    {
        private readonly DataContext context;
        private Random random;

        public SeedDb(DataContext context)
        {
            this.context = context;
            this.random = new Random();
        }

        public async Task SeedAsync()
        {
            await this.context.Database.EnsureCreatedAsync();

            if (!this.context.Clients.Any())
            {
                this.AddClient("Alejandro Newberry M", "20362132283");
                this.AddClient("NYO SOFT S.R.L.", "30714414581");
                this.AddClient("Adrian Toledo", "2022453943");
                await this.context.SaveChangesAsync();
            }
        }

        private void AddClient(string name, string CUIT)
        {
            this.context.Clients.Add(new Client
            {
                Name = name,
                CUIT = CUIT,
                Email = "prueba@gmail.com",
                Phone = "0303456"
            });
        }
    }
}