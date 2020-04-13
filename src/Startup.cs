using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
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
            services.AddDbContext<CompanyDbContext>();
            services.TryAddSingleton(provider =>
                new DbContextOptionsBuilder<CompanyDbContext>()
                    .UseSqlServer(Configuration.GetConnectionString("DefaultConnection")).Options
            );

            services.AddTransient<EmployeesRepository>();

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

    public class DbInitializer
    {
        public static async Task Initialize(CompanyDbContext context, CancellationToken ct = default)
        {
            await context.Database.EnsureCreatedAsync(ct);
        }
    }
}
