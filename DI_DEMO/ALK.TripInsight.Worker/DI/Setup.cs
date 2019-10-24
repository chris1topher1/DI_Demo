using ALK.TripInsight.Worker.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ALK.TripInsight.StopStatusWorker")]
namespace ALK.TripInsight.Worker.DI
{
    internal static class Setup
    {
        private static readonly AutoResetEvent _closing = new AutoResetEvent(false);

        public static void Configure(
            IServiceProvider serviceProvider,
            IConfigurationRoot configuration,
            string workerName)
        {
            Task.Factory.StartNew(() =>
            {
                serviceProvider.GetService<IGPSProcessor>().ReadQueue();

                var cancellationTokenSource = new CancellationTokenSource();

                Console.WriteLine("Press enter to stop the process!");

                serviceProvider.GetService<IHostedService>().StartAsync(cancellationTokenSource.Token);
            });
            Console.CancelKeyPress += _onExit;
            _closing.WaitOne();
        }

        private static void _onExit(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("Exit");
            _closing.Set();
        }
    }
}