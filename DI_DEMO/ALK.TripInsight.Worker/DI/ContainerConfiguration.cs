using ALK.TripInsight.Worker.Date;
using ALK.TripInsight.Worker.RabbitMQ;
using ALK.TripInsight.Worker.Status;
using ALK.TripInsight.Worker.Trip.StopInfo;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using ALK.TripInsight.Worker.GPS;
using Microsoft.Extensions.Hosting;

namespace ALK.TripInsight.Worker.DI
{
    internal static class ContainerConfiguration
    {
        private const string QUEUE_END_POINT = "ConnectionStrings:QueueEndpointURL";
        private const string CUSTOM_DB_CONNECTION_STRING_KEY = "ConnectionStrings:CustomDb";

        internal static IServiceProvider Configure(IConfiguration configuration)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddNLog();
            });

            var containerBuilder = new ContainerBuilder();

            containerBuilder.Populate(serviceCollection);

            //
            // Register dependencies here
            //

            containerBuilder.RegisterType<GpsQueueControllerService>().As<IHostedService>().SingleInstance();

            var container = containerBuilder.Build();
            var serviceProvider = new AutofacServiceProvider(container);

            return serviceProvider;
        }
    }
}