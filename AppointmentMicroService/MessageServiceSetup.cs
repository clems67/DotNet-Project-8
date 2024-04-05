using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using AppointmentMicroService.Controllers;

namespace AppointmentMicroService
{
    public class MessageServiceSetup
    {

        public void Setup()
        {
            ConnectionFactory factory = new ConnectionFactory();
            //factory.Uri = new Uri("amqp://guest:guest@localhost:5672/");
            factory.ClientProvidedName = "Rabbit Receiver Microservice App";

            IConnection connection = factory.CreateConnection();

            factory.UserName = "guest";
            factory.Password = "guest";
            factory.HostName = "localhost";
            factory.Port = 5672;
            factory.ClientProvidedName = "Rabbit  Receiver Microservice  App";

            IModel channel = connection.CreateModel();

            string exchangeName = "CalifornianHealthExchange";
            //string routingKey = "DemoRoutingKey";
            string queueName = "AppointmentQueue";

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, false, false, false, null);
            //channel.QueueBind(queueName, exchangeName, routingKey, null);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (sender, args) =>
            {
                 var body = args.Body.ToArray();

                string message = Encoding.UTF8.GetString(body);

                AppointmentController appointmentController = new AppointmentController();
                if(args.RoutingKey == "GET_APPOINTMENT")
                {
                    await appointmentController.GetAppointment();
                }

                channel.BasicAck(args.DeliveryTag, false);
            };
            string consumerTag = channel.BasicConsume(queueName, false, consumer);

            //channel.BasicCancel(consumerTag);
            //channel.Close();
            //conn.Close();
        }
    }
}
