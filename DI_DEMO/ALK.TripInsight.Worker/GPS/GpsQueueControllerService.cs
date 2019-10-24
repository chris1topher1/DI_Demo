using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ALK.TripInsight.Worker.GPS
{
    internal class GpsQueueControllerService : IHostedService
    {
        private readonly ILogger<GpsQueueControllerService> _logger;

        public GpsQueueControllerService(ILogger<GpsQueueControllerService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                _logger.LogInformation("GpsQueueControllerService started!");

                while (!cancellationToken.IsCancellationRequested)
                {
                }
            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("GpsQueueControllerService stopped!");
            return Task.CompletedTask;
        }
    }
}