using BaseCap.Hosting.HealthChecks;
using BaseCap.Hosting.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;

namespace BaseCap.Hosting.Extensions
{
    /// <summary>
    /// Extension class for adding health probes
    /// </summary>
    public static class HealthCheckServiceExtension
    {
        private static PlatformHealthCheckOptions? _options;

        /// <summary>
        /// Adds health probes to the process
        /// </summary>
        /// <param name="services">The Service Collection to add to</param>
        /// <param name="options">The health probe options used to configure the probes</param>
        /// <returns>Returns the Service Collection for chaining</returns>
        public static IServiceCollection AddPlatformHealthChecks(this IServiceCollection services, PlatformHealthCheckOptions options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            else if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options;

            IHealthChecksBuilder builder = services.AddHealthChecks();

            if (options.AddBasicLivenessProbe)
            {
                services.AddSingleton<ILivenessQueryResponder, BasicLivenessQueryResponder>();
                builder.AddCheck<LivenessProbe>("liveness", failureStatus: HealthStatus.Unhealthy, tags: new[] { "live" });
            }

            if (options.AddBasicReadinessProbe || options.AddCustomReadinessProbe)
            {
                services.AddSingleton<ReadinessProbe>();
                builder.AddCheck<ReadinessProbe>("readiness", failureStatus: HealthStatus.Unhealthy, tags: new[] { "ready" });
            }

            if (options.AddBasicReadinessProbe)
            {
                services.AddHostedService<BasicReadinessService>();
            }

            return services;
        }

        /// <summary>
        /// Provides the Health Probes to the process
        /// </summary>
        /// <param name="app">The App Builder for the process</param>
        /// <returns>Returns the App Builder for chaining</returns>
        public static IApplicationBuilder UsePlatformHealthChecks(this IApplicationBuilder app)
        {
            if (_options == null)
            {
                throw new InvalidOperationException("Must Add Health Checks before configuring them");
            }

            if (_options.AddBasicLivenessProbe || (_options.AddBasicReadinessProbe || _options.AddCustomReadinessProbe))
            {
                app.UseEndpoints(endpoints =>
                    {
                        if (_options.AddBasicLivenessProbe)
                        {
                            endpoints.MapHealthChecks("/health/live", new HealthCheckOptions()
                            {
                                AllowCachingResponses = false,
                                Predicate = (check) => check.Tags.Contains("live"),
                            });
                        }

                        if (_options.AddBasicReadinessProbe || _options.AddCustomReadinessProbe)
                        {
                            endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions()
                            {
                                AllowCachingResponses = false,
                                Predicate = (check) => check.Tags.Contains("ready"),
                            });
                        }
                    });
            }

            return app;
        }
    }
}
