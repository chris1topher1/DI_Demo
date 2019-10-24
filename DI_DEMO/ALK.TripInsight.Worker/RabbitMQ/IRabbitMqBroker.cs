using System;
using System.Collections.Generic;
using ALK.TripInsight.Model;

namespace ALK.TripInsight.Worker.RabbitMQ
{
    internal interface IRabbitMqBroker : IDisposable
    {
        void Init(string url, string queueName);

        void Subscribe(Func<string, IDictionary<string, object>, bool> callback);
    }

    internal interface IDistanceCalculator
    {
        double CalculateDistance(TripStopInfo stop, GpsMessage gpsMessage);
    }
}