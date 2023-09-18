
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Entities
{
    public class WebContext : DbContext
    {
        public DbSet<Worker> Workers { set; get; }
        public DbSet<OrderHistory> OrderHistories { set; get; }
        public DbSet<HouseHoldChores> HouseHoldChores { set; get; }
        public WebContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Workers_Chores>(entity =>
            {
                entity.HasKey(e => new { e.WorkerId, e.ChoreId });

            });
        }

    }
}