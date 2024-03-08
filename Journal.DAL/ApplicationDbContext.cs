using Microsoft.EntityFrameworkCore;
using Journal.Domain.Models;

namespace Journal.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasOne(u => u.Subscription).WithOne(ci => ci.User).HasForeignKey<Subscription>(ci => ci.UserId);
            modelBuilder.Entity<MTAccount>().HasOne(u => u.User).WithMany(u => u.MTAccounts).HasForeignKey(u => u.UserID).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<CTraderAccount>().HasOne(u => u.User).WithMany(ci => ci.CTraderAccounts).HasForeignKey(ci => ci.UserID).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Deal>().HasOne(u =>u.Account).WithMany(ci => ci.Deals).HasForeignKey(ci => ci.AccountId).OnDelete(DeleteBehavior.NoAction);        }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<MTAccount> MTAccounts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Deal> MTDeals { get; set; }
        public DbSet<CTraderAccount> CTraderAccounts { get; set;}
    }
}
