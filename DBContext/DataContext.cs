using APITask.Models;
using Microsoft.EntityFrameworkCore;

namespace APITask.DBContext
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
           : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "User" },
                new Role { Id = 2, Name = "Admin" },
                new Role { Id = 3, Name = "Support" },
                new Role { Id = 4, Name = "SuperAdmin" }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "John Doe",
                    Age = 30,
                    Email = "john.doe@example.com"
                },
                new User
                {
                    Id = 2,
                    Name = "Jane Smith",
                    Age = 25,
                    Email = "jane.smith@example.com"
                },
                new User
                {
                    Id = 3,
                    Name = "Alice Johnson",
                    Age = 35,
                    Email = "alice.johnson@example.com"
                },
                new User
                {
                    Id = 4,
                    Name = "Bob Wilson",
                    Age = 40,
                    Email = "bob.wilson@example.com"
                },
                new User
                {
                    Id = 5,
                    Name = "Eve Brown",
                    Age = 28,
                    Email = "eve.brown@example.com"
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}