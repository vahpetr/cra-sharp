using Backend.Data.Models;
using Microsoft.EntityFrameworkCore;

// https://docs.microsoft.com/ru-ru/ef/core/miscellaneous/cli/dotnet
namespace Backend.Data {
    public class UsersContext : DbContext {
        public UsersContext (DbContextOptions<UsersContext> options) : base (options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            modelBuilder.Entity<User> ()
                .HasAlternateKey (c => c.Email);
        }
    }
}