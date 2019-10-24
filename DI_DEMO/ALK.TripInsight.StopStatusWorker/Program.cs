using ALK.TripInsight.Worker.Configuration;
using ALK.TripInsight.Worker.DI;
using Microsoft.Extensions.Configuration;
using System;
using ALK.TripInsight.Worker.GPS;
using ALK.TripInsight.Worker.RabbitMQ;

namespace ALK.TripInsight.StopStatusWorker
{
    class Program
    {
        public static void Main(string[] args)
        {
            GPSProcessor processor = new GPSProcessor();
            processor.ReadQueue();

            // Wait until user presses ESC
            Console.WriteLine("Press ESC to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
