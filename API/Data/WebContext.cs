
using System.ComponentModel.DataAnnotations;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Entities
{
    public class WebContext : DbContext
    {
        public DbSet<Worker> Workers { set; get; }
        public DbSet<OrderHistory> OrderHistories { set; get; }
        public DbSet<HouseHoldChores> HouseHoldChores { set; get; }
        public DbSet<Workers_Chores> Workers_Chores { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<TrackingWorker> TrackingWorker { get; set; }
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

            modelBuilder.Entity<Worker>(entity =>
            {
                entity.HasOne(w => w.User)
                .WithOne(u => u.Worker)
                .HasForeignKey<Worker>(w => w.Id)
                .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasOne(r => r.OrderHistory)
                .WithOne(o => o.Review)
                .HasForeignKey<Review>(r => r.Id)
                .OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<TrackingWorker>(entity =>
            {
                entity.HasKey(e => new { e.WorkerId, e.ChoreId });
            });
        }
    }
}