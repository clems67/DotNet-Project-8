using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web;

namespace CalifornianHealthMonolithic
{
    public class RpcClient : IDisposable
    {
        private readonly string exchangeName = string.Empty;
        protected string QUEUE_NAME;

        private IConnection connection;
        private IModel channel;
        private string replyQueueName;
        private ConcurrentDictionary<string, TaskCompletionSource<string>> callbackMapper = new ConcurrentDictionary<string, TaskCompletionSource<string>>();
        internal CommunicationModel communicationModel;

        public RpcClient(string queueName)
        {
            Debug.WriteLine("\nSetup connection\n");
            QUEUE_NAME = queueName;
            var factory = new ConnectionFactory { HostName = "localhost" };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            // declare a server-named queue
            replyQueueName = channel.QueueDeclare().QueueName;
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                if (!callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out var tcs))
                    return;
                var body = ea.Body.ToArray();
                var responseString = Encoding.UTF8.GetString(body);
                try
                {
                    communicationModel = JsonConvert.DeserializeObject<CommunicationModel>(responseString);
                }
                catch (Exception ex) {
                    Debug.WriteLine(ex.Message);
                }
                tcs.TrySetResult(responseString);
            };

            channel.BasicConsume(consumer: consumer,
                                 queue: replyQueueName,
                                 autoAck: false);
        }

        public Task<string> CallAsync(CommunicationModel communicationModel, CancellationToken cancellationToken = default)
        {
            IBasicProperties props = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            var message = JsonConvert.SerializeObject(communicationModel);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var tcs = new TaskCompletionSource<string>();
            callbackMapper.TryAdd(correlationId, tcs);

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: QUEUE_NAME,
                                 basicProperties: props,
                                 body: messageBytes);

            cancellationToken.Register(() => callbackMapper.TryRemove(correlationId, out _));
            return tcs.Task;
        }

        public void Dispose()
        {
            channel.Close();
            connection.Close();
        }
    }
}