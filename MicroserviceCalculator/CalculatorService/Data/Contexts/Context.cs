using CalculatorService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalculatorService.Data.Contexts
{
    public class Context : DbContext
    {
        public Context() { }

        // Specify a default location for the SQLite database
        private const string DefaultDbPath = "WannaBeFirmaSQLite.db";

        // Define a DbSet for your Result entity
        public DbSet<Result> Results { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // Always use the default location for the SQLite database
            options.UseSqlite($"Data Source={DefaultDbPath}");
        }
    }
}
