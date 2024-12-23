using Microsoft.EntityFrameworkCore;
using TaskManagement.Models;

namespace TaskManagement.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Tasks> Tasks { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
    }
}
