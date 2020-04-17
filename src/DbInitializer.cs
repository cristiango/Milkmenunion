using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MilkmenUnion.Storage;

namespace MilkmenUnion
{
    /// <summary>
    /// Ideally this should run as part of bootstrapping the application.
    /// Consider some kind of pipeline where we return 503(or a `degraded` state) until we initialize our dependencies. Ok 503 is not ideal in AWS/Azure but you get the gist
    /// </summary>
    public class DbInitializer
    {
        private readonly ILogger<DbInitializer> _logger;
        private readonly CompanyDbContext _dbContext;

        public DbInitializer(ILogger<DbInitializer> logger, CompanyDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        public async Task Initialize(CancellationToken ct = default)
        {
            //apply possible migrations also
            await _dbContext.Database.EnsureCreatedAsync(ct);

            var pendingMigrations =
                (await _dbContext.Database.GetPendingMigrationsAsync(CancellationToken.None)).ToArray();

            var appliedMigrations = await _dbContext.Database.GetAppliedMigrationsAsync(CancellationToken.None);
            appliedMigrations.ForEach(migration =>
                _logger.LogInformation("{migration} migration already applied", migration));

            pendingMigrations.ForEach(migration => _logger.LogInformation("{migration} pending", migration));

            await _dbContext.Database.MigrateAsync(cancellationToken: ct);

            pendingMigrations.ForEach(migration => _logger.LogInformation("{migration} applied", migration));
        }
    }
}