using Microsoft.EntityFrameworkCore;

namespace MilkmenUnion.Domain
{
    public class EmployeesDbContext: DbContext
    {
        public EmployeesDbContext(DbContextOptions<EmployeesDbContext> options) : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entry => { entry.HasKey(p => p.Id); });
            base.OnModelCreating(modelBuilder);
        }
    }
}