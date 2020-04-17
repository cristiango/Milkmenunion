using System;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MilkmenUnion
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var build = CreateHostBuilder(args).Build();

            //HACK Alert!! replace this with IHostedService or at least with a proper CommandLine module
            if (args.FirstOrDefault() == "db") //hehe
            {
                using var scope = build.Services.CreateScope();
                var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();

                if (args.Any(x => x == "init"))
                {
                    Console.WriteLine("Check database initialization");
                    //lets initialize the database
                    dbInitializer.Initialize(CancellationToken.None).GetAwaiter().GetResult();
                }

                if (args.Any(x => x == "drop"))
                {
                    dbInitializer.Drop(CancellationToken.None).GetAwaiter().GetResult();
                }

                return;
            }

            
            build.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
