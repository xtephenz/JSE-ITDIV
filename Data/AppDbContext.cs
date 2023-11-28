using System;
using JSE.Models;
using Microsoft.EntityFrameworkCore;

namespace JSE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        { }
        public DbSet<Delivery> Delivery { get; set; }
        public DbSet<Courier> Courier { get; set; }
        public DbSet<PoolBranch> PoolBranch { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Message> Message { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the relationships
            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.SenderPool)
                .WithMany(p => p.SendingDeliveries)
                .HasForeignKey(d => d.pool_sender_city)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.ReceiverPool)
                .WithMany(p => p.ReceivingDeliveries)
                .HasForeignKey(d => d.pool_receiver_city)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }

}

