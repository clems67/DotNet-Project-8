using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using Shared;
using System.Net.Http.Json;

namespace AppointmentMicroservice
{
    public class MessageServiceSetup : Shared.MessageServiceSetup
    {
        public IAppointmentService AppointmentService { get; set; }
        public MessageServiceSetup(IServiceProvider serviceProvider) : base("Appointment_queue")
    {
            this.AppointmentService = serviceProvider.GetService<IAppointmentService>();
        }
        //public ConnectionFactory _connectionFactory = new ConnectionFactory
        //{
        //    HostName = "localhost",
        //    RequestedHeartbeat = TimeSpan.Parse("10"),
        //    AutomaticRecoveryEnabled = true
        //};
        //public IConnection _connection { get; set; }
        //public IModel _channel { get; set; }

        public override CommunicationModel MessageHandler(CommunicationModel questionModel)
        {
            var response = new CommunicationModel(){};

            if (questionModel.AccessTypeSelected == CommunicationModel.AccessType.getAppointments)
            {
                response.Appointments = AppointmentService.GetRecentAppointments(questionModel.ConsultantIdSelected);
            }
            else if (questionModel.AccessTypeSelected == CommunicationModel.AccessType.createNewAppointment)
            {
                response.IsAppointmentsCreated = AppointmentService.CreateAppointment(questionModel.AppointmentToCreate);                
            }
            else
            {
                response.AccessTypeSelected = CommunicationModel.AccessType.error;
            }
            return response;
        }

        //public async void Setup()
        //{
        //    _connection = _connectionFactory.CreateConnection();

        //    _channel = _connection.CreateModel();

        //    _channel.QueueDeclare(queue: "Appointment_queue",
        //                         durable: false,
        //                         exclusive: false,
        //                         autoDelete: false,
        //                         arguments: null);

        //    _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        //    var consumer = new EventingBasicConsumer(_channel);
        //    _channel.BasicConsume(queue: "Appointment_queue",
        //        autoAck: false,
        //        consumer: consumer);
        //    Debug.WriteLine("\n Connection microservice rabbitmq\n");

        //    consumer.Received += async (model, ea) =>
        //    {
        //        Debug.WriteLine("\nmicroservice called\n");
        //        var body = ea.Body.ToArray();
        //        Debug.WriteLine($"body: " + body);
        //        Debug.WriteLine("routing key: " + ea.RoutingKey);
        //        var props = ea.BasicProperties;
        //        var replyProps = _channel.CreateBasicProperties();
        //        replyProps.CorrelationId = props.CorrelationId;
        //        _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

        //        string response = string.Empty;
        //        try
        //        {
        //            var questionString = Encoding.UTF8.GetString(body);
        //            var question = JsonSerializer.Deserialize<CommunicationModel>(questionString);
        //            response = JsonSerializer.Serialize(MessageHandler(question));
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.WriteLine($"\nERROR MICROSERVICE\n [.] {e.Message}");
        //            response = JsonSerializer.Serialize(
        //                new CommunicationModel()
        //                {
        //                    AccessTypeSelected = CommunicationModel.AccessType.error
        //                }
        //                );
        //        }
        //        finally
        //        {
        //            try
        //            {

        //                var responseBytes = Encoding.UTF8.GetBytes(response);
        //                _channel.BasicPublish(exchange: string.Empty,
        //                                     routingKey: props.ReplyTo,
        //                                     basicProperties: replyProps,
        //                                     body: responseBytes);

        //            }
        //            catch (Exception e)
        //            {
        //                Debug.WriteLine("\nERROR REPLYTO\n");
        //            }
        //        }
        //    };
        //}
    }
}
