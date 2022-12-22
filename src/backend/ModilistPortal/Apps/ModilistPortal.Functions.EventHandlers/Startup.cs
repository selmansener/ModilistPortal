using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ModilistPortal.Functions.EventHandlers.Configurations;
using ModilistPortal.Infrastructure.Azure.Extensions;
using ModilistPortal.Infrastructure.Shared.Configurations;

using Newtonsoft.Json;

[assembly: FunctionsStartup(typeof(ModilistPortal.Functions.EventHandlers.Startup))]
namespace ModilistPortal.Functions.EventHandlers
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var environment = builder.Services.BuildServiceProvider().GetService<IHostEnvironment>();
            var options = GetConfigurations(environment.EnvironmentName);

            builder.Services.Configure<StorageConnectionStrings>(config =>
            {
                config.AppStorage = options.AppStorage;
            });

            builder.Services.AddBlobClientFactory();

            builder.Services.BuildServiceProvider();
        }

        public ConfigurationOptions GetConfigurations(string environmentName)
        {
            var assembly = typeof(Startup).Assembly;
            using (var stream = assembly.GetManifestResourceStream($"ModilistPortal.Functions.EventHandlers.Settings.appsettings.{environmentName}.json"))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string options = reader.ReadToEnd();
                if (string.IsNullOrEmpty(options))
                {
                    throw new InvalidOperationException($"Could not load appsettings file.");
                }

                return JsonConvert.DeserializeObject<ConfigurationOptions>(options);
            }
        }
    }
}
