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
            builder.Entity<UserChallenge>().HasKey(sc => new { sc.UserId, sc.ChallengeId });
            builder.Entity<UserChallenge>()
               .HasOne(pt => pt.User)
               .WithMany(p => p.UserChallenges)
               .HasForeignKey(pt => pt.UserId);

            builder.Entity<UserChallenge>()
                .HasOne(pt => pt.Challenge)
                .WithMany(t => t.UserChallenges)
                .HasForeignKey(pt => pt.ChallengeId);

            builder.Entity<UserCupon>().HasKey(sc => new { sc.UserId, sc.CuponId });
            builder.Entity<UserCupon>()
               .HasOne(pt => pt.User)
               .WithMany(p => p.UserCupons)
               .HasForeignKey(pt => pt.UserId);

            builder.Entity<UserCupon>()
                .HasOne(pt => pt.Cupon)
                .WithMany(t => t.UserCupons)
                .HasForeignKey(pt => pt.CuponId);
        }

        public virtual DbSet<Challenge> Challenges { get; set; }
        public virtual DbSet<Cupon> Cupons { get; set; }
        public virtual DbSet<UserChallenge> UserChallenges { get; set; }
        public virtual DbSet<UserCupon> UserCupons { get; set; }

    }
}
