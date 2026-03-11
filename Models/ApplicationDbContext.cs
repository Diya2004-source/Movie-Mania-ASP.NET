// Models/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using MovieMania.Models;

namespace MovieMania.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Change DbSet<User> to DbSet<AppUser>
        public DbSet<AppUser> Users { get; set; }  // This is the key change

        public DbSet<Movie> Movies { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Referral> Referrals { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<MovieReview> MovieReviews { get; set; }
        public DbSet<ShowReview> ShowReviews { get; set; }
        public DbSet<EpisodeComment> EpisodeComments { get; set; }
        public DbSet<WishlistShow> WishlistShows { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Update all relationships to use AppUser
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany(u => u.Wishlists)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserSubscription>()
                .HasOne(us => us.User)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

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

            // Add other configurations...
        }
    }
}