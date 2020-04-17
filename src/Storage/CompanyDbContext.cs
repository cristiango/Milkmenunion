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

        public DbSet<SalaryInformation> Salaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entry =>
            {
                entry.HasKey(p => p.Id);
                entry.HasMany<SalaryInformation>()
                    .WithOne(s => s.Employee)
                    .HasForeignKey(s => s.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade); //GDPR we clean up everything
            });

            modelBuilder.Entity<SalaryInformation>(entry =>
            {
                entry.HasKey(x => x.Id);
                entry.Property(p => p.Id).ValueGeneratedOnAdd();
                    entry
                        .HasOne(salaryInformation => salaryInformation.Employee)
                        .WithMany(employee => employee.SalaryHistory)
                        .HasForeignKey(salaryInformation => salaryInformation.EmployeeId)
                        .IsRequired();
                });
            base.OnModelCreating(modelBuilder);
        }
    }
}