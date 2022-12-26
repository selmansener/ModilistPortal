using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ModilistPortal.Functions.BlobTriggered.Configurations;
using ModilistPortal.Infrastructure.Azure.Extensions;
using ModilistPortal.Infrastructure.Shared.Configurations;

using Newtonsoft.Json;

[assembly: FunctionsStartup(typeof(ModilistPortal.Functions.BlobTriggered.Startup))]
namespace ModilistPortal.Functions.BlobTriggered
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var options = GetConfigurations(env);

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
            using (var stream = assembly.GetManifestResourceStream($"ModilistPortal.Functions.BlobTriggered.Settings.appsettings.{environmentName}.json"))
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
