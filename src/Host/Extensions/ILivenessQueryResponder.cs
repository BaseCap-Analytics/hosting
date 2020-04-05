using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace BaseCap.Hosting.Extensions
{
    /// <summary>
    /// Binding for querying for process liveness
    /// </summary>
    public interface ILivenessQueryResponder
    {
        /// <summary>
        /// Callback function called when the process has been asked for its liveness
        /// </summary>
        /// <param name="context">The context with the Liveness check</param>
        /// <param name="cancellationToken">A CancellationToken to notify when the check has been cancelled</param>
        /// <returns>Returns the HealthCheckResult to respond with</returns>
        Task<HealthCheckResult> QueryLivenessAsync(HealthCheckContext context, CancellationToken cancellationToken);
    }
}
