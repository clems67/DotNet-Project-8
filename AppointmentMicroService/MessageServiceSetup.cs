using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;

namespace AppointmentMicroservice
{
    public class MessageServiceSetup
    {
        public IAppointmentService AppointmentController { get; set; }
        public MessageServiceSetup(IServiceProvider serviceProvider)
        {
            this.AppointmentController = serviceProvider.GetService<IAppointmentService>();
        }

        private string _queueName = "Appointment_queue";
        public ConnectionFactory _connectionFactory = new ConnectionFactory
        {
            HostName = "localhost",
            RequestedHeartbeat = TimeSpan.Parse("10"),
            AutomaticRecoveryEnabled = true
        };
        public IConnection _connection { get; set; }
        public IModel _channel { get; set; }

        public AppointmentCommunicationModel MessageHandler(AppointmentCommunicationModel questionModel)
        {
            if (questionModel.AccessTypeSelected == AppointmentCommunicationModel.AccessType.getAppointments)
            {
                questionModel.Appointments = AppointmentController.GetRecentAppointments(questionModel.ConsultantIdSelected);
            }
            else if (questionModel.AccessTypeSelected == AppointmentCommunicationModel.AccessType.createNewAppointment)
            {
                questionModel.IsAppointmentsCreated = AppointmentController.CreateAppointment(questionModel.AppointmentToCreate);
            }
            else
            {
                questionModel.AccessTypeSelected = AppointmentCommunicationModel.AccessType.error;
            }
            return questionModel;
        }

        public async void Setup()
        {
            _connection = _connectionFactory.CreateConnection();

            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            var consumer = new EventingBasicConsumer(_channel);
            _channel.BasicConsume(queue: _queueName,
                autoAck: false,
                consumer: consumer);
            Debug.WriteLine("\n Connection microservice rabbitmq\n");

            consumer.Received += async (model, ea) =>
            {
                Debug.WriteLine("\nmicroservice called\n");
                var body = ea.Body.ToArray();
                Debug.WriteLine($"body: " + body);
                Debug.WriteLine("routing key: " + ea.RoutingKey);
                var props = ea.BasicProperties;
                var replyProps = _channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                string response = string.Empty;
                try
                {
                    var questionString = Encoding.UTF8.GetString(body);
                    var question = Newtonsoft.Json.JsonConvert.DeserializeObject<AppointmentCommunicationModel>(questionString);
                    var responseObject = MessageHandler(question);
                    response = JsonConvert.SerializeObject(responseObject);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"\nERROR MICROSERVICE\n [.] {e.Message}");
                    response = JsonConvert.SerializeObject(
                        new AppointmentCommunicationModel()
                        {
                            AccessTypeSelected = AppointmentCommunicationModel.AccessType.error
                        }
                        );
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
