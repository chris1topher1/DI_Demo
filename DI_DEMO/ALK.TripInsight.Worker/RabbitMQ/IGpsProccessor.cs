using System.Collections.Generic;

namespace ALK.TripInsight.Worker.RabbitMQ
{
    internal interface IGPSProcessor
    {
        void ReadQueue();
    }
}