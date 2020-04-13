using Microsoft.EntityFrameworkCore;
using MilkmenUnion.Domain;

namespace MilkmenUnion.Storage
{
    public class CompanyDbContext: DbContext
    {
        public CompanyDbContext(DbContextOptions<CompanyDbContext> options) : base(options)
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