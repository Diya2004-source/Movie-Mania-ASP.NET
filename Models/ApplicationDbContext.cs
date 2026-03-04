using Microsoft.EntityFrameworkCore;
using movie.Models;
using MovieMania.Models;

namespace MovieMania.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Referral> Referrals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============================
            // Referral Configuration
            // ============================
            modelBuilder.Entity<Referral>()
                .HasOne(r => r.Referrer)
                .WithMany(u => u.ReferralsMade)
                .HasForeignKey(r => r.ReferrerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Referral>()
                .HasOne(r => r.ReferredUser)
                .WithMany(u => u.ReferralsReceived)
                .HasForeignKey(r => r.ReferredUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================
            // Wishlist Configuration
            // ============================
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany(u => u.Wishlists)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.Movie)
                .WithMany()
                .HasForeignKey(w => w.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============================
            // UserSubscription Configuration
            // ============================
            modelBuilder.Entity<UserSubscription>()
                .HasOne(us => us.User)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserSubscription>()
                .HasOne(us => us.SubscriptionPlan)
                .WithMany()
                .HasForeignKey(us => us.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============================
            // Payment Configuration
            // ============================
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============================
            // Decimal Precision (Important)
            // ============================
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<SubscriptionPlan>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Referral>()
                .Property(r => r.RewardAmount)
                .HasColumnType("decimal(18,2)");
        }
    }
}