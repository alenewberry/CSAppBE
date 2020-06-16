﻿namespace CSAppBE.Web.Data
{
    using CSAppBE.Web.Data.Entities;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;

    public class DataContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
    }
}