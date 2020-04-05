using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BaseCap.Hosting.GlobalConfiguration.Extensions
{
    /// <summary>
    /// Provides access to the flag indicating if the service is running as Production Mode or not
    /// </summary>
    public static class ProductionSwitchProviderExtension
    {
        private const string ASPNETCORE_PRODUCTION_NAME = "ASPNETCORE_ENVIRONMENT";
        private const string PRODUCTION_FLAG_NAME = "production";

        /// <summary>
        /// Provides a flag indicating if the Service is running in Production Mode or not
        /// </summary>
        /// <param name="services">The Service Collection to add to</param>
        /// <param name="configuration">The configuration settings to use</param>
        /// <returns>Returns true if the Service is running in Production Mode; otherwise, returns false</returns>
        public static bool GetIsProduction(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            else if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            string? aspEnvironment = configuration[ASPNETCORE_PRODUCTION_NAME];
            return InternalGetIsProduction(aspEnvironment);
        }

        /// <summary>
        /// Provides a flag indicating if the Service is running in Production Mode or not
        /// </summary>
        /// <returns>Returns true if the Service is running in Production Mode; otherwise, returns false</returns>
        public static bool GetIsProduction()
        {
            string? aspEnvironment = Environment.GetEnvironmentVariable(ASPNETCORE_PRODUCTION_NAME);
            return InternalGetIsProduction(aspEnvironment);
        }

        private static bool InternalGetIsProduction(string? aspEnvironment) => string.Equals(aspEnvironment, PRODUCTION_FLAG_NAME, StringComparison.OrdinalIgnoreCase);
    }
}
