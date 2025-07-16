using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Projet> Projets { get; set; }
        // Tu ajouteras d'autres DbSet ici (ex. Utilisateurs, Organisations, etc.)
    }
}
