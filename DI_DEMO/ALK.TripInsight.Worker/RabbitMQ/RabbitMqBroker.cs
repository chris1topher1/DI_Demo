using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("ALK.TripInsight.Worker.Test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace ALK.TripInsight.Worker.RabbitMQ
{
    internal class RabbitMqBroker : IRabbitMqBroker
    {
        private ConnectionFactory _connectionFactory;
        private IConnection _conn;
        private IModel _channel;
        private string _queueName;

        /// <summary>
        /// Initializes the broker to the specified queue
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="queueName">Name of the queue.</param>
        /// <exception cref="System.Exception">Broker already initialized</exception>
        public void Init(string url, string queueName)
        {
            // connect to server
            _connectionFactory = new ConnectionFactory();
            _connectionFactory.Uri = new Uri(url);
            _conn = _connectionFactory.CreateConnection();

            // create queue (if necessary)
            _channel = _conn.CreateModel();
            _channel.QueueDeclare(queueName, true, false, false, null);
            _queueName = queueName;
        }

        /// <summary>
        /// Begin the listening for messages
        /// </summary>
        /// <param name="callback">The callback function to execute when a message is received.</param>
        public void Subscribe(Func<string, IDictionary<string, object>, bool> callback)
        {
            // create consumer to read message and invoke a callback
            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, args) =>
            {
                string messageBody = Encoding.UTF8.GetString(args.Body);

                bool success = callback.Invoke(messageBody, args.BasicProperties.Headers);

                // if processing was successful, remove the message from the queue
                if (success)
                {
                    _channel.BasicAck(args.DeliveryTag, true);
                }
                else
                {
                    //_logger.LogInformation($"Message {messageBody} was not processed, sending back to queue!");
                }
            };

            // begin consuming
            _channel.BasicConsume(_queueName, false, consumer);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _conn?.Close();
            _conn?.Dispose();
        }
    }
}
