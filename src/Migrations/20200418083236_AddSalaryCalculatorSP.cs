using Microsoft.EntityFrameworkCore.Migrations;

namespace MilkmenUnion.Migrations
{
    public partial class AddSalaryCalculatorSP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NeedsInitialSalaryCalculation",
                table: "Employees",
                nullable: false,
                defaultValue: false);
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_aproximateSalary 
    
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[Salaries]
           ([MonthlyAmount]
           ,[ChangeDate]
           ,[EmployeeId])

    Select (e.Height * 5 + DATEDIFF(year, e.DateOfBirth, GETDATE()) * 8)/1.7 + 1800 salary, GETDATE() as changeDate, e.Id 
    from dbo.Employees e 
    where e.NeedsInitialSalaryCalculation = 1

     --do we have a race condition here? Update only ones that have the salary already calculated
     --transaction not absolutely needed in here since this store procedure is idempotent. Retry policy should be build in the domain when running this
    Update Employees set NeedsInitialSalaryCalculation = 0 
    From Employees e 
        left outer join Salaries s on s.EmployeeId = e.Id
    where s.Id is null and NeedsInitialSalaryCalculation = 1
END
GO
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NeedsInitialSalaryCalculation",
                table: "Employees");

            migrationBuilder.Sql("drop procedure sp_aproximateSalaries");
        }
    }
}
