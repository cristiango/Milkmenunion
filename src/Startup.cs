using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MilkmenUnion.Controllers.Models.Validators;
using MilkmenUnion.Domain;
using MilkmenUnion.Storage;

namespace MilkmenUnion
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<GetUtcNow>(SystemClock.Default);
            services.AddDbContext<CompanyDbContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                });
            
            //Not enough time to tend to integration tests. Disable this bit in order to use `dotnet-ef migrations add InitialCreate --project .\src\MilkmenUnion.csproj`
            //For future developer use a separated SqlCompanyDbContext that should be used only when adding new ef migrations

            //services.TryAddSingleton(provider =>
            //    new DbContextOptionsBuilder<CompanyDbContext>()
            //        .UseSqlServer(Configuration.GetConnectionString("DefaultConnection")).Options
            //);

            services.AddTransient<EmployeesRepository>();

            //Validators
            services.AddSingleton<CreateEmployeeRequestValidator>();

            services.AddHealthChecks();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseHealthChecks("/health");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    /// <summary>
    /// Ideally this should run as part of bootstrapping the application.
    /// Consider some kind of pipeline where we return 503(or a `degraded` state) until we initialize our dependencies. Ok 503 is not ideal in AWS/Azure but you get the gist
    /// </summary>
    public class DbInitializer
    {
        public static async Task Initialize(CompanyDbContext dbContext, CancellationToken ct = default)
        {
            //apply possible migrations also
            await dbContext.Database.EnsureCreatedAsync(ct);

            var pendingMigrations =
                (await dbContext.Database.GetPendingMigrationsAsync(CancellationToken.None)).ToArray();

            var appliedMigrations = await dbContext.Database.GetAppliedMigrationsAsync(CancellationToken.None);
            appliedMigrations.ForEach(migration =>
                _logger.LogInformation("{migration} migration already applied", migration));

            pendingMigrations.ForEach(migration => _logger.LogInformation("{migration} pending", migration));

            await dbContext.Database.MigrateAsync();

            pendingMigrations.ForEach(migration => _logger.LogInformation("{migration} applied", migration));
        }
    }
}
