using Gateway.Models;
using Microsoft.AspNetCore.Connections;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace Gateway
{
    public interface IAppointmentRpcClient
    {
        public AppointmentCommunicationModel appointmentCommunicationModel { get; set; }
        public Task<string> CallAppointmentMicroserviceAsync(AppointmentCommunicationModel communicationModel, CancellationToken cancellationToken = default);
    }
    public interface IConsultantRpcClient
    {
        public ConsultantCommunicationModel consultantCommunicationModel { get; set; }
        public Task<string> CallConsultantMicroserviceAsync(ConsultantCommunicationModel communicationModel, CancellationToken cancellationToken = default);
    }
    public class RpcClient : IDisposable, IAppointmentRpcClient, IConsultantRpcClient
    {
        private readonly string exchangeName = string.Empty;
        protected string QUEUE_NAME;

        private IConnection connection;
        private IModel channel;
        private string replyQueueName;
        private ConcurrentDictionary<string, TaskCompletionSource<string>> callbackMapper = new ConcurrentDictionary<string, TaskCompletionSource<string>>();
        public AppointmentCommunicationModel appointmentCommunicationModel { get; set; }
        public ConsultantCommunicationModel consultantCommunicationModel { get; set; }

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
                    appointmentCommunicationModel = JsonConvert.DeserializeObject<AppointmentCommunicationModel>(responseString);
                }
                catch (Exception ex1)
                {
                    try
                    {
                        consultantCommunicationModel = JsonConvert.DeserializeObject<ConsultantCommunicationModel>(responseString);
                    }
                    catch (Exception ex2)
                    {
                        Debug.WriteLine(ex1.Message);
                        Debug.WriteLine(ex2.Message);
                    }
                }
                tcs.TrySetResult(responseString);
            };

            channel.BasicConsume(consumer: consumer,
                                 queue: replyQueueName,
                                 autoAck: false);
        }

        public Task<string> CallAppointmentMicroserviceAsync(AppointmentCommunicationModel communicationModel, CancellationToken cancellationToken = default)
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

        public Task<string> CallConsultantMicroserviceAsync(ConsultantCommunicationModel communicationModel, CancellationToken cancellationToken = default)
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
