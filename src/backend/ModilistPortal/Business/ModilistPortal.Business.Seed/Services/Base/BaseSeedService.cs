using System.Collections.Immutable;

using ModilistPortal.Business.Seed.Configuration;
using ModilistPortal.Data.DataAccess;

namespace ModilistPortal.Business.Seed.Services.Base
{
    internal interface ISeedService
    {
        Task Execute(CancellationToken cancellationToken);

        ImmutableList<SeedServiceType> GetDependencies();
    }

    internal abstract class BaseSeedService : ISeedService
    {
        protected readonly ModilistPortalDbContext _dbContext;

        protected BaseSeedService(ModilistPortalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected virtual ImmutableList<SeedServiceType> Dependencies { get; } = ImmutableList<SeedServiceType>.Empty;

        public abstract Task Execute(CancellationToken cancellationToken);

        public ImmutableList<SeedServiceType> GetDependencies()
        {
            return Dependencies;
        }
    }
}
