using Microsoft.EntityFrameworkCore;
using WaitingLineRestaurant.API.Entities;

namespace WaitingLineRestaurant.API
{
    public class WaitingLineRestaurantContext : DbContext
    {
        public WaitingLineRestaurantContext(DbContextOptions<WaitingLineRestaurantContext> options) : base(options)
        {

        }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Phone).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}