using System;
using JSE.Models;
using Microsoft.EntityFrameworkCore;

namespace JSE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
        public DbSet<Delivery> Delivery { get; set; }
        public DbSet<Courier> Courier { get; set; }
        public DbSet<PoolBranch> PoolBranch { get; set; }
        public DbSet<Admin> Admin { get; set; }

    }

}

