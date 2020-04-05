using BaseCap.Hosting.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace BaseCap.Hosting.Services
{
    /// <summary>
    /// Basic liveness responder that always says OK
    /// </summary>
#pragma warning disable CA1812 // This class isn't directly created but rather created through dependency injection
    internal class BasicLivenessQueryResponder : ILivenessQueryResponder
    {
        /// <inheritdoc/>
        public Task<HealthCheckResult> QueryLivenessAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(HealthCheckResult.Healthy("Live"));
        }
    }
#pragma warning restore CA1812 // This class isn't directly created but rather created through dependency injection
}
