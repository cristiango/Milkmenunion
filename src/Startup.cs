using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MilkmenUnion.Controllers.Models.Validators;
using MilkmenUnion.Domain;
using MilkmenUnion.Import;
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
            services.AddTransient<DbInitializer>();
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
            services.AddTransient<FileImporter>();
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
}
