using Microsoft.EntityFrameworkCore;
using Journal.Domain.Models;

namespace Automarket.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>().HasMany(u => u.Accounts).WithOne(ci => ci.User).HasForeignKey(ci => ci.UserId);
            modelBuilder.Entity<User>().HasOne(u => u.Subscription).WithOne(ci => ci.User).HasForeignKey<Subscription>(ci => ci.UserId);
            modelBuilder.Entity<MTAccount>().HasMany(u => u.Deals).WithOne(ci => ci.Account).HasForeignKey(ci => ci.AccountId);
        }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<MTAccount> MTAccounts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<MTDeal> MTDeals { get; set; }
    }
}
