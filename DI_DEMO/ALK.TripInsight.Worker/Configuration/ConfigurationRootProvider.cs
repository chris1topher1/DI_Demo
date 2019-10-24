using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.IO;

namespace ALK.TripInsight.Worker.Configuration
{
    internal static class ConfigurationRootProvider
    {
        public static IConfigurationRoot GetConfiguration()
        {
            var environmentName = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT") ?? "dev";

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                .Build();

            // load NLog config
            var nlogConfigFileName = "nlog.config";
            var environmentSpecificLogFileName = $"nlog.{environmentName}.config";
            if (File.Exists(environmentSpecificLogFileName))
            {
                nlogConfigFileName = environmentSpecificLogFileName;
            }

            LogManager.LoadConfiguration(nlogConfigFileName);

            Console.WriteLine($"Loaded environment: {environmentName}");

            return configuration;
        }
    }
}
