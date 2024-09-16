using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;
using System.CodeDom;

namespace BulkyBook.Data
{
    public class ApplicationDbContext : DbContext
    {

        public DbSet<Category> Categories { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var cats = new Category[] {
                new Category { Id = 1, Name="Action", DisplayOrder = 1 },
                new Category { Id = 2, Name="SciFi", DisplayOrder = 2 },
                new Category { Id = 3, Name="Comedy", DisplayOrder = 3 },
                new Category { Id = 4, Name="Thriller", DisplayOrder = 4 },
                new Category { Id = 5, Name="Horror", DisplayOrder = 5 },
            };
            modelBuilder.Entity<Category>().HasData(cats);
        }
    }
}
