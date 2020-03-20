using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EmployeeManagement
{
    public class AppDBContext : IdentityDbContext<ApplicationUser>
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    Name = "Mary",
                    Email = "mary@gmail.com",
                    Department = Dept.HR
                },
                new Employee
                {
                    Id = 2,
                    Name = "John",
                    Email = "john@gmail.com",
                    Department = Dept.IT
                },
                new Employee
                {
                    Id = 3,
                    Name = "Mark",
                    Email = "mark@gmail.com",
                    Department = Dept.IT
                }
            );

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
