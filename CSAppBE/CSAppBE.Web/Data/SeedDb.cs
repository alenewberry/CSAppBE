namespace CSAppBE.Web.Data
{
    using Helpers;
    using Entities;
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class SeedDb
    {
        private readonly DataContext context;
        private readonly IUserHelper userHelper;
        private Random random;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            this.context = context;
            this.userHelper = userHelper;
            this.random = new Random();
        }

        public async Task SeedAsync()
        {
            await this.context.Database.EnsureCreatedAsync();

            var user = await this.userHelper.GetUserByEmailAsync("alenewberry@gmail.com");
            if (user == null)
            {
                user = new User
                {
                    Name = "Alejandro Newberry",
                    Serial = "3001",
                    Email = "alenewberry@gmail.com",
                    UserName = "alenewberry@gmail.com"
                };

                var result = await this.userHelper.AddUserAsync(user, "123456");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }
            }         


            if (!this.context.Clients.Any())
            {
                this.AddClient("Alejandro Newberry M", "20362132283", user);
                this.AddClient("NYO SOFT S.R.L.", "30714414581", user);
                this.AddClient("Adrian Toledo", "2022453943", user);
                await this.context.SaveChangesAsync();
            }
        }

        private void AddClient(string name, string CUIT, User user)
        {
            this.context.Clients.Add(new Client
            {
                Name = name,
                CUIT = CUIT,
                Email = "prueba@gmail.com",
                Phone = "0303456",
                User = user
            });
        }
    }
}