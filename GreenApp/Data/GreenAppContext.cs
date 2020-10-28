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

        public DbSet<Task> Task { get; set; }
    }
}
