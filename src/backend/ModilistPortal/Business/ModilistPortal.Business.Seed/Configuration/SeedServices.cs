using ModilistPortal.Business.Seed.Services.Base;

namespace ModilistPortal.Business.Seed.Configuration
{
    internal sealed class SeedServices : Dictionary<string, Type>
    {
        public ISeedService GetService(IServiceProvider serviceProvider, SeedServiceType service)
        {
            var serviceName = service.ToString();
            if (!ContainsKey(serviceName))
            {
                throw new InvalidOperationException($"Invalid seed service: {serviceName}");
            }

            return (ISeedService)serviceProvider.GetService(this[serviceName]);
        }
    }
}
