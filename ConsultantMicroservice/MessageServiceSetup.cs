using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ConsultantMicroservice
{
    public class MessageServiceSetup
    {
        public IConsultantService ConsultantController { get; set; }
        public MessageServiceSetup(IServiceProvider serviceProvider)
        {
            this.ConsultantController = serviceProvider.GetService<IConsultantService>();
        }
        public ConnectionFactory _connectionFactory = new ConnectionFactory
        {
            HostName = "localhost",
            RequestedHeartbeat = System.TimeSpan.Parse("10"),
            AutomaticRecoveryEnabled = true
        };
        public IConnection _connection { get; set; }
        public IModel _channel { get; set; }
        public async void Setup()
        {
            _connection = _connectionFactory.CreateConnection();

            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "rpc_queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            var consumer = new EventingBasicConsumer(_channel);
            _channel.BasicConsume(queue: "rpc_queue",
                autoAck: false,
                consumer: consumer);
            Debug.WriteLine("\n Connection microservice rabbitmq\n");

            consumer.Received += async (model, ea) =>
            {
                Debug.WriteLine("\nmicroservice called\n");
                
                string response = string.Empty;

                var body = ea.Body.ToArray();
                Debug.WriteLine($"body: " + body);
                var props = ea.BasicProperties;
                var replyProps = _channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                try
                {
                    response = JsonSerializer.Serialize(await ConsultantController.GetAppointment());

                }
                catch (Exception e)
                {
                    Debug.WriteLine($"\nERROR MICROSERVICE\n [.] {e.Message}");
                    response = string.Empty;
                }
                finally
                {
                    try
                    {

                        var responseBytes = Encoding.UTF8.GetBytes(response);
                        _channel.BasicPublish(exchange: string.Empty,
                                             routingKey: props.ReplyTo,
                                             basicProperties: replyProps,
                                             body: responseBytes);

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("\nERROR REPLYTO\n");
                    }

                }


            };
        }


    }
}
