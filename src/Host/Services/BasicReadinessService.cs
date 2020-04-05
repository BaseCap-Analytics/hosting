using BaseCap.Hosting.HealthChecks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace BaseCap.Hosting.Services
{
    /// <summary>
    /// Service that toggles a Readiness Probe upon running
    /// </summary>
#pragma warning disable CA1812 // This class isn't directly created but rather created through dependency injection
    internal class BasicReadinessService : BackgroundService
    {
        private readonly ReadinessProbe _readyProbe;

        /// <summary>
        /// Creates a new Basic Readiness Service that will announce that the service has completed initialization
        /// </summary>
        /// <param name="readyProbe">The Readiness Probe to toggle</param>
        public BasicReadinessService(ReadinessProbe readyProbe)
        {
            _readyProbe = readyProbe;
        }

        /// <inheritdoc />
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _readyProbe.Ready = true;
            return Task.CompletedTask;
        }
    }
#pragma warning restore CA1812 // This class isn't directly created but rather created through dependency injection
}
