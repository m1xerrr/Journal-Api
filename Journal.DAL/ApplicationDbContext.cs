using Microsoft.EntityFrameworkCore;
using Journal.Domain.Models;

namespace Journal.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasOne(u => u.Subscription).WithOne(ci => ci.User).HasForeignKey<Subscription>(ci => ci.UserId);
            modelBuilder.Entity<MTAccount>().HasOne(u => u.User).WithMany(u => u.MTAccounts).HasForeignKey(u => u.UserID).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<CTraderAccount>().HasOne(u => u.User).WithMany(ci => ci.CTraderAccounts).HasForeignKey(ci => ci.UserID).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<DXTradeAccount>().HasOne(u => u.User).WithMany(ci => ci.DXTradeAccounts).HasForeignKey(ci => ci.UserID).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Deal>().HasOne(u =>u.Account).WithMany(ci => ci.Deals).HasForeignKey(ci => ci.AccountId).OnDelete(DeleteBehavior.NoAction);        
            modelBuilder.Entity<Note>().HasOne(u => u.User).WithMany(ci => ci.Notes).HasForeignKey(ci => ci.UserId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<DescriptionItem>().HasOne(u => u.Deal).WithMany(ci => ci.DescriptionItems).HasForeignKey(ci => ci.DealId).OnDelete(DeleteBehavior.NoAction);
        }

        public DbSet<DXTradeAccount> DXTradeAccounts { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<MTAccount> MTAccounts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Deal> Deals { get; set; }
        public DbSet<CTraderAccount> CTraderAccounts { get; set;}
        public DbSet<Note> Notes { get; set; }

        public DbSet<DescriptionItem> DealsDescriptions { get; set; }
    }
}
