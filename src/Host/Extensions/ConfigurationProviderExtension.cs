using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BaseCap.Hosting.Extensions
{
    /// <summary>
    /// Provides access to the Configuration Root
    /// </summary>
    public static class ConfigurationProviderExtension
    {
        /// <summary>
        /// Provides a Configuration Root
        /// </summary>
        /// <param name="services">The Service Collection to add to</param>
        /// <param name="configuration">The configuration settings to use</param>
        /// <returns>Returns the Service Collection for chaining</returns>
        public static IServiceCollection UseConfigurationRoot(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            else if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.AddSingleton<IConfiguration>(configuration);

            return services;
        }
    }
}
