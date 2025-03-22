using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Shoell.Autobody.Data.Tests
{
    public class DatabaseInitializationService(AutobodyContext context) : IHostedService
    {
        protected AutobodyContext Context => context;

        public virtual async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await SeedProjectDataAsync(cancellationToken);
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await DropProjectDatabaseAsync(cancellationToken);
        }

        private async Task DropProjectDatabaseAsync(CancellationToken cancellationToken = default)
        {
            await Context.Database.EnsureDeletedAsync(cancellationToken);
        }

        private async Task SeedProjectDataAsync(CancellationToken cancellationToken = default)
        {
            await Context.Database.EnsureDeletedAsync(cancellationToken);
            await Context.Database.MigrateAsync(cancellationToken);
        }
    }
}
