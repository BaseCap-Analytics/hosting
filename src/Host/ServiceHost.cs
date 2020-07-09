using BaseCap.Hosting.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace BaseCap.Hosting
{
    /// <summary>
    /// The base host for running Cloud Services
    /// </summary>
    public abstract class ServiceHost
    {
        private readonly IHostBuilder _hostBuilder;
        private readonly string _serviceName;
        private readonly ushort _listenPort;
        private IHost? _host;

        /// <summary>
        /// Creates a new Cloud Service
        /// </summary>
        /// <param name="serviceName">The name of the Cloud Service being run</param>
        /// <param name="listenPort">The port the service is listening on</param>
        public ServiceHost(string serviceName, ushort listenPort)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                throw new ArgumentNullException(nameof(serviceName));
            }
            else if (listenPort < 80)
            {
                throw new ArgumentOutOfRangeException(nameof(listenPort));
            }

            _serviceName = serviceName;
            _listenPort = listenPort;
            _host = null;
            _hostBuilder = new HostBuilder()
                            .ConfigureWebHost(WebSetup)
                            .UseConsoleLifetime();
        }

        private void ConfigureAppConfigurationSettings(IConfigurationBuilder config)
        {
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? Directory.GetCurrentDirectory();
            IConfiguration secrets = GetConfiguration();
            config.AddConfiguration(secrets);
            config.SetBasePath(path);
        }

        /// <summary>
        /// Retrieves the current Configuration settings
        /// </summary>
        /// <returns>Returns the current Configuration settings</returns>
        public abstract IConfiguration GetConfiguration();

        private void WebSetup(IWebHostBuilder webBuilder)
        {
            bool isProduction = ProductionSwitchProviderExtension.GetIsProduction();
            string url = isProduction ? "http://0.0.0.0" : "https://127.0.0.1";
            webBuilder.UseKestrel()
                .UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .UseUrls($"{url}:{_listenPort}")
                .ConfigureAppConfiguration(ConfigureAppConfigurationSettings)
                .ConfigureServices(ConfigureServices)
                .Configure(ConfigureWeb);
        }

        /// <summary>
        /// Add custom services in the Cloud Service
        /// </summary>
        /// <param name="services">The Service Collection to add services to</param>
        /// <param name="configuration">The current Configuration of the Cloud Service</param>
        public abstract void AddCustomServices(IServiceCollection services, IConfiguration configuration);

        /// <summary>
        /// Configure any Web Services for the Cloud Service
        /// </summary>
        /// <param name="app">The Web Application Builder to configure</param>
        public abstract void ConfigureCustomServices(IApplicationBuilder app);

        private void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
        {
            IConfiguration configuration = context.Configuration;
            services.AddSingleton<IConfiguration>(configuration);

            // Always add response compression
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            // Add routing
            services.Configure<KestrelServerOptions>(options =>
                {
                    options.AllowSynchronousIO = true;
                })
                .AddRouting();

            // Enable forwarded headers
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
            });

            AddCustomServices(services, configuration);
        }

        private void ConfigureWeb(IApplicationBuilder app)
        {
            app.UseForwardedHeaders()
               .UseResponseCompression()
               .UseRouting();

            ConfigureCustomServices(app);
        }

        /// <summary>
        /// Starts the Cloud Service
        /// </summary>
        /// <returns>Returns an awaitable Task</returns>
        public async Task StartAsync()
        {
            _host = _hostBuilder.Build();

            try
            {
                string versionNumber = Assembly.GetExecutingAssembly().GetName().Version!.ToString();
                Log.Logger.Information("Starting Service {Name} with version {Version} at {Time}", _serviceName, versionNumber, DateTimeOffset.UtcNow);
                await _host.RunAsync().ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // No-op since this is thrown when Ctrl+C is pressed or the console window is closed
            }
            catch (Exception ex)
            {
                // Out platform logger should be initialized to use that one and our initialized one
                Log.Logger.Error(ex, "Uncaught error in service {Name}", _serviceName);
            }
            finally
            {
                Log.Logger.Information("Stopping Service {Name} at {Time}", _serviceName, DateTimeOffset.UtcNow);
            }
        }
    }
}
