namespace ALK.TripInsight.Worker.RabbitMQ
{
    internal interface IStopStatusQueueReader
    {
        void ReadQueue();
    }
}