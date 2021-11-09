using Microsoft.EntityFrameworkCore;
using System;

namespace test_santa_api {
    public class SHDbContext: DbContext
    {
        public DbSet<Child> Clients {get;set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("DB_CONNSTR"));
        }
    } 
}