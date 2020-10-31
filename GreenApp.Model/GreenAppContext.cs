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
            // A felhasználói tábla alapértelmezett neve AspNetUsers lenne az adatbázisban, de ezt felüldefiniálhatjuk.
        }


        public virtual DbSet<Challange> Challanges { get; set; }

    }
}
