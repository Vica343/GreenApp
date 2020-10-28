using Microsoft.EntityFrameworkCore;
using GreenApp.Models;

namespace GreenApp.Data
{
    public class GreenAppContext : DbContext
    {
        public GreenAppContext(DbContextOptions<GreenAppContext> options)
            : base(options)
        {
        }

        public DbSet<Challange> Challange { get; set; }
    }
}
