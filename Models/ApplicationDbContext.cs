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

        // DbSets
        public DbSet<AppUser> Users { get; set; }
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

            // ============================
            // Offer - SubscriptionPlan Relationship
            // ============================
            modelBuilder.Entity<Offer>()
                .HasOne(o => o.SubscriptionPlan)
                .WithMany()
                .HasForeignKey(o => o.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // ============================
            // Movie - Genre Relationship
            // ============================
            modelBuilder.Entity<Movie>()
                .HasOne(m => m.GenreNavigation)
                .WithMany(g => g.Movies)
                .HasForeignKey(m => m.GenreId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // ============================
            // Show - Genre Relationship (FIXED)
            // ============================
            modelBuilder.Entity<Show>()
                .HasOne(s => s.GenreNavigation)
                .WithMany(g => g.Shows)
                .HasForeignKey(s => s.GenreId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);  // Explicitly mark as optional

            // ============================
            // Show - Episodes Relationship
            // ============================
            modelBuilder.Entity<Episode>()
                .HasOne(e => e.Show)
                .WithMany(s => s.Episodes)
                .HasForeignKey(e => e.ShowId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============================
            // User Relationships
            // ============================
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

            // ============================
            // MovieReview Relationships
            // ============================
            modelBuilder.Entity<MovieReview>()
                .HasOne(mr => mr.Movie)
                .WithMany(m => m.Reviews)
                .HasForeignKey(mr => mr.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MovieReview>()
                .HasOne(mr => mr.User)
                .WithMany(u => u.MovieReviews)
                .HasForeignKey(mr => mr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============================
            // ShowReview Relationships
            // ============================
            modelBuilder.Entity<ShowReview>()
                .HasOne(sr => sr.Show)
                .WithMany(s => s.Reviews)
                .HasForeignKey(sr => sr.ShowId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ShowReview>()
                .HasOne(sr => sr.User)
                .WithMany(u => u.ShowReviews)
                .HasForeignKey(sr => sr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================
            // WishlistShow Relationships
            // ============================
            modelBuilder.Entity<WishlistShow>()
                .HasOne(ws => ws.User)
                .WithMany(u => u.WishlistShows)
                .HasForeignKey(ws => ws.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WishlistShow>()
                .HasOne(ws => ws.Show)
                .WithMany(s => s.WishlistShows)
                .HasForeignKey(ws => ws.ShowId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============================
            // EpisodeComment Relationships
            // ============================
            modelBuilder.Entity<EpisodeComment>()
                .HasOne(ec => ec.Episode)
                .WithMany(e => e.Comments)
                .HasForeignKey(ec => ec.EpisodeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EpisodeComment>()
                .HasOne(ec => ec.User)
                .WithMany(u => u.EpisodeComments)
                .HasForeignKey(ec => ec.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EpisodeComment>()
                .HasOne(ec => ec.ParentComment)
                .WithMany(ec => ec.Replies)
                .HasForeignKey(ec => ec.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================
            // UserActivity Relationships
            // ============================
            modelBuilder.Entity<UserActivity>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.Activities)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserActivity>()
                .HasOne(ua => ua.Movie)
                .WithMany(m => m.UserActivities)
                .HasForeignKey(ua => ua.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserActivity>()
                .HasOne(ua => ua.Episode)
                .WithMany(e => e.UserActivities)
                .HasForeignKey(ua => ua.EpisodeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================
            // Decimal Precision Configurations
            // ============================
            modelBuilder.Entity<Movie>()
                .Property(m => m.Rating)
                .HasPrecision(3, 1);

            modelBuilder.Entity<MovieReview>()
                .Property(m => m.Rating)
                .HasPrecision(3, 1);

            modelBuilder.Entity<Show>()
                .Property(s => s.Rating)
                .HasPrecision(3, 1);

            modelBuilder.Entity<ShowReview>()
                .Property(s => s.Rating)
                .HasPrecision(3, 1);

            modelBuilder.Entity<Wishlist>()
                .Property(w => w.UserRating)
                .HasPrecision(3, 1);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SubscriptionPlan>()
                .Property(sp => sp.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Referral>()
                .Property(r => r.RewardAmount)
                .HasPrecision(18, 2);
        }
    }
}