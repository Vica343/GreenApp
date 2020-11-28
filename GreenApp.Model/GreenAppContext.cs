using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GreenApp.Model
{
    public class GreenAppContext : IdentityDbContext<Guest, IdentityRole<int>, int>
    {
        public GreenAppContext(DbContextOptions<GreenAppContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Guest>().ToTable("Guests");

            builder.Entity<UserChallenge>()
                  .HasKey(bc => new { bc.UserId, bc.ChallengeId });

            builder.Entity<UserChallenge>()
                .HasOne(bc => bc.User)
                .WithMany(b => b.UserChallenges)
                .HasForeignKey(bc => bc.UserId);

            builder.Entity<UserChallenge>()
               .HasOne(bc => bc.Challenge)
               .WithMany(b => b.UserChallenges)
               .HasForeignKey(bc => bc.ChallengeId);

            builder.Entity<UserCupon>()
                 .HasKey(bc => new { bc.UserId, bc.CuponId });
            builder.Entity<UserCupon>()
                .HasOne(bc => bc.User)
                .WithMany(b => b.UserCupons)
                .HasForeignKey(bc => bc.UserId);

            builder.Entity<UserCupon>()
               .HasOne(bc => bc.Cupon)
               .WithMany(b => b.UserCupons)
               .HasForeignKey(bc => bc.CuponId);

            builder.Entity<Guest>()
                .HasMany(c => c.CreatedChallenges)
                .WithOne(g => g.Creator)
                .HasForeignKey(k => k.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public virtual DbSet<Challenge> Challenges { get; set; }
        public virtual DbSet<Cupon> Cupons { get; set; }
        public virtual DbSet<UserChallenge> UserChallenges { get; set; }
        public virtual DbSet<UserCupon> UserCupons { get; set; }
        public virtual DbSet<Nonprofit> Nonprofits { get; set; }

    }
}
