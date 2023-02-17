
using System.Globalization;
using System.Net;

using Mapster;

using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility.EventCounterCollector;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

using ModilistPortal.API.AuthorizationPolicies;
using ModilistPortal.API.Configuration;
using ModilistPortal.API.Middlewares;
using ModilistPortal.API.SwaggerConfiguration.Filters;
using ModilistPortal.Business.Extensions;
using ModilistPortal.Business.Seed;
using ModilistPortal.Business.Utils.Extensions;
using ModilistPortal.Data.Extensions;
using ModilistPortal.Infrastructure.Azure.Extensions;
using ModilistPortal.Infrastructure.Azure.Extensions.Configurations;
using ModilistPortal.Infrastructure.Shared.Configurations;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

internal class Program
{
    private static void Main(string[] args)
    {
        const string CorsPolicyName = "Default";
        const string ApiTitle = "ModilistPortalAPI";
        const string SwaggerEndpoint = "/swagger/v1/swagger.json";

        var builder = WebApplication.CreateBuilder(args);

        var appSettings = builder.Configuration.GetSection("AppSettings");
        var config = appSettings.Get<ConfigurationOptions>();

        IHostEnvironment environment = builder.Environment;


        builder.Services.Configure<ConfigurationOptions>(configuration =>
        {
            configuration.DevelopmentApiKey = config.DevelopmentApiKey;
            configuration.AllowedOrigins = config.AllowedOrigins;
            configuration.AzureAdB2COptions = config.AzureAdB2COptions;
            configuration.ModilistDbConnectionOptions = config.ModilistDbConnectionOptions;
        });

        builder.Services.Configure<IyzicoAPIOptions>(builder.Configuration.GetSection("AppSettings:IyzicoAPIOptions"));
        builder.Services.Configure<StorageConnectionStrings>(builder.Configuration.GetSection("AppSettings:StorageConnectionStrings"));
        builder.Services.Configure<SendGridOptions>(builder.Configuration.GetSection("AppSettings:SendGridOptions"));
        builder.Services.Configure<EventGridClientOptions>(builder.Configuration.GetSection("AppSettings:EventGridClientOptions"));

        builder.Services.AddMemoryCache();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddBusinessUtils();

        builder.Services.AddBlobClientFactory();

        builder.Services.AddHttpContextAccessor();

        TypeAdapterConfig.GlobalSettings.Scan(typeof(Program).Assembly, typeof(BusinessExtensions).Assembly);
        TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);

        builder.Services.AddCors(ConfigureCors);


        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(options =>
            {
                builder.Configuration.Bind("AzureAdB2C", options);

                options.TokenValidationParameters.NameClaimType = "name";
            },
            options => { builder.Configuration.Bind("AzureAdB2C", options); });

        builder.Services.AddControllers();

        builder.Services.AddHttpsRedirection(options =>
        {
            options.RedirectStatusCode = (int)HttpStatusCode.PermanentRedirect;
            options.HttpsPort = 443;
        });

        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = false;

        builder.Services.AddAuthorization(options =>
        {
            var permissions = GetPermissions();

            foreach (var permission in permissions)
            {
                options.AddPolicy(permission.Key, policy => policy.Requirements.Add(new ScopesRequirement(permission.Value)));
            }
        });


        builder.Services.AddApplicationInsightsTelemetry(options =>
        {
            options.ConnectionString = config.AppInsightsConnectionString;

            if (environment.IsDevelopment())
            {
                options.DeveloperMode = true;
            }
        });

        builder.Services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) =>
        {
            module.EnableRequestIdHeaderInjectionInW3CMode = true;
            module.EnableSqlCommandTextInstrumentation = true;
            module.EnableAzureSdkTelemetryListener = true;
            module.EnableSqlCommandTextInstrumentation = true;
            module.EnableW3CHeadersInjection = true;
        });

        builder.Services.ConfigureTelemetryModule<PerformanceCollectorModule>((module, o) =>
        {
        });

        builder.Services.ConfigureTelemetryModule<RequestTrackingTelemetryModule>((module, o) =>
        {
            module.CollectionOptions.InjectResponseHeaders = true;
            module.CollectionOptions.TrackExceptions = true;
            module.CollectionOptions.EnableW3CDistributedTracing = true;
        });

        builder.Services.ConfigureTelemetryModule<EventCounterCollectionModule>((module, o) =>
        {
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "time-in-gc"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "alloc-rate"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "cpu-usage"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "exception-count"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "gc-heap-size"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "gen-0-gc-count"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "gen-0-size"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "gen-1-gc-count"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "gen-1-size"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "gen-2-gc-count"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "gen-2-size"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "loh-size"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "poh-size"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "gc-fragmentation"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "monitor-lock-contention-count"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "active-timer-count"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "assembly-count"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "threadpool-completed-items-count"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "threadpool-queue-length"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "threadpool-thread-count"));
            module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "working-set"));
        });

        builder.Services.AddRepositories();

        builder.Services.AddCQRS();

        builder.Services.AddCors(ConfigureCors);

        builder.Services.AddDataAccess(config.ModilistDbConnectionOptions, builder.Environment);

        builder.Services.AddSeedServices(builder.Environment.EnvironmentName);

        var mvcBuilder = builder.Services.AddMvc(ConfigureMvc)
            .AddNewtonsoftJson(ConfigureNewtonsoftJson)
            .ConfigureApiBehaviorOptions(ConfigureApiBehavior);


        mvcBuilder.AddValidations();

        builder.Services.AddTransactionManager();

        builder.Services.AddLoggingBehavior();
        builder.Services.AddValidationBehavior();
        builder.Services.AddTransactionBehavior();

        builder.Services.AddSwaggerGenNewtonsoftSupport();
        builder.Services.AddSwaggerGen(ConfigureSwaggerGenerator);

        builder.Services.AddApiVersioning(ConfigureApiVersioning);

        builder.Services.AddVersionedApiExplorer(ConfigureApiExplorer);

        var app = builder.Build();

        app.UseCors(CorsPolicyName);

        app.UseHttpsRedirection();
        app.UseSwagger();
        app.UseSwaggerUI(ConfigureSwaggerUI);
        app.UseStaticFiles();

        app.UseMiddleware<TelemetryMiddleware>();

        app.UseExceptionHandlerMiddleware();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.Run();

        void ConfigureCors(CorsOptions obj)
        {
            obj.AddPolicy(CorsPolicyName, builder =>
            {
                builder.WithOrigins(config.AllowedOrigins.ToArray());
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowCredentials();
            });
        }

        void ConfigureSwaggerUI(SwaggerUIOptions options)
        {
            options.SwaggerEndpoint(SwaggerEndpoint, ApiTitle);
            options.DocExpansion(DocExpansion.None);
            options.DisplayRequestDuration();
            options.OAuthClientId(config.AzureAdB2COptions.ClientId);
            options.InjectJavascript("https://code.jquery.com/jquery-3.6.0.min.js");
            options.InjectJavascript("../js/swagger-seed-dropdown-sorting.js");
        }

        void ConfigureSwaggerGenerator(SwaggerGenOptions options)
        {
            options.SupportNonNullableReferenceTypes();
            options.OperationFilter<ResolveDynamicQueryEndpoints>("dqb");

            options.OperationFilter<ApiKeyHeaderParameterOperationFilter>();

            options.SwaggerDoc("v1", new OpenApiInfo { Title = ApiTitle, Version = "v1" });
            options.CustomSchemaIds(DefaultSchemaIdSelector);
            var permissions = GetPermissions().ToDictionary(
                x => $"https://{config.AzureAdB2COptions.Domain}/{config.AzureAdB2COptions.ClientId}/{x.Value}",
                x => x.Key);

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.OAuth2,
                Scheme = "Bearer",
                OpenIdConnectUrl = new Uri(config.AzureAdB2COptions.OpenIdConnectUrl, UriKind.Absolute),
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(config.AzureAdB2COptions.AuthorizationUrl, UriKind.Absolute),
                        Scopes = permissions
                    }
                }
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                            Type = SecuritySchemeType.OAuth2,
                            OpenIdConnectUrl = new Uri(config.AzureAdB2COptions.OpenIdConnectUrl, UriKind.Absolute),
                            Flows = new OpenApiOAuthFlows
                            {
                                Implicit = new OpenApiOAuthFlow
                                {
                                    AuthorizationUrl = new Uri(config.AzureAdB2COptions.AuthorizationUrl, UriKind.Absolute),
                                    Scopes = permissions
                                }
                            }
                        },
                        new List<string>()
                    }
                });

            var enabledAreas = new List<string>
    {
        "api"
    };

            if (!builder.Environment.IsProduction())
            {
                enabledAreas.Add("dev");
            }

            options.DocumentFilter<SwaggerAreaFilter>(new object[] { enabledAreas.ToArray() });
        }

        string DefaultSchemaIdSelector(Type modelType)
        {
            if (!modelType.IsConstructedGenericType)
            {
                return modelType.Name;
            }

            string prefix = modelType.GetGenericArguments()
                .Select(genericArg => DefaultSchemaIdSelector(genericArg))
                .Aggregate((previous, current) => previous + current);

            return prefix + modelType.Name.Split('`').First();
        }

        void ConfigureMvc(MvcOptions options)
        {
            // This line adds a caching profile to use in controllers or actions.
            options.CacheProfiles.Add("Default", new CacheProfile { Duration = -1, Location = ResponseCacheLocation.None, NoStore = true });
            // This line adds default cache profile to all controllers as a filter.
            options.Filters.Add(new ResponseCacheAttribute { CacheProfileName = "Default" });
        }

        void ConfigureNewtonsoftJson(MvcNewtonsoftJsonOptions options)
        {
            options.SerializerSettings.ContractResolver =
                  new CamelCasePropertyNamesContractResolver();
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ss.FFFZ";
        }

        void ConfigureApiBehavior(ApiBehaviorOptions options)
        {
            // SuppressModelStateInvalidFilter prevents automatically returning HttpStatus 400 when the ModelState is not valid.
            // The reason to preventing this is throwing a ValidationException to customize the response with a special format.
            // Related code can be found in ReportModelValidationErrorsFilter.
            options.SuppressModelStateInvalidFilter = true;
        }

        void ConfigureApiVersioning(ApiVersioningOptions options)
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        }

        void ConfigureApiExplorer(ApiExplorerOptions options)
        {
            // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            // note: the specified format code will format the version as "'v'major[.minor][-status]"
            options.GroupNameFormat = "'v'V";

            // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
            // can also be used to control the format of the API version in route templates
            options.SubstituteApiVersionInUrl = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
        }

        IDictionary<string, string> GetPermissions()
        {
            var authPermissions = new AuthorizationPermissions();
            var fields = typeof(AuthorizationPermissions).GetFields();
            var permissions = new Dictionary<string, string>();

            foreach (var field in fields)
            {
                if (!permissions.ContainsKey(field.Name))
                {
                    permissions.Add(field.Name, field.GetValue(permissions).ToString());
                }
            }

            return permissions;
        }
    }
}