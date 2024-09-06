using CalifornianHealthMonolithic.Code;
using CalifornianHealthMonolithic.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CalifornianHealthMonolithic.Controllers
{
    public class BookingController : Controller
    {
        // GET: Booking
        //TODO: Change this method to display the consultant calendar. Ensure that the user can have a real time view of 
        //the consultant's availability;
        public ActionResult GetConsultantCalendar()
        {
            ConsultantModelList conList = new ConsultantModelList();
            CHDBContext dbContext = new CHDBContext();
            Repository repo = new Repository();
            List<Consultant> cons = new List<Consultant>();
            cons = repo.FetchConsultants(dbContext);
            conList.ConsultantsList = new SelectList(cons, "Id", "FName");
            conList.consultants = cons;

            var rpcClient = new RpcClient();

            var response = rpcClient.CallAsync();

            return View(conList);
        }

        public class RpcClient : IDisposable
        {
            private readonly string exchangeName = string.Empty;
            private readonly string routingKey = "GET_APPOINTMENT";
            private const string QUEUE_NAME = "rpc_queue";

            private IConnection connection;
            private IModel channel;
            private string replyQueueName;
            private ConcurrentDictionary<string, TaskCompletionSource<string>> callbackMapper = new ConcurrentDictionary<string, TaskCompletionSource<string>>();

            public RpcClient()
            {
                Debug.WriteLine("\nSetup connection\n");
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
                    var response = Encoding.UTF8.GetString(body);
                    Debug.WriteLine($"the response in controller : {response}");
                    tcs.TrySetResult(response);
                };

                channel.BasicConsume(consumer: consumer,
                                     queue: replyQueueName,
                                     autoAck: false);
            }

            public Task<string> CallAsync(CancellationToken cancellationToken = default)
            {
                Debug.WriteLine("\nCallAsync\n");
                IBasicProperties props = channel.CreateBasicProperties();
                var correlationId = Guid.NewGuid().ToString();
                props.CorrelationId = correlationId;
                props.ReplyTo = replyQueueName;
                var messageBytes = Encoding.UTF8.GetBytes("message that has been setup in BookingController");
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

        //TODO: Change this method to ensure that members do not have to wait endlessly. 
        public ActionResult ConfirmAppointment(Appointment model)
        {
            CHDBContext dbContext = new CHDBContext();

            //Code to create appointment in database
            //This needs to be reassessed. Before confirming the appointment, should we check if the consultant calendar is still available?
            Repository repo = new Repository();
            var result = repo.CreateAppointment(model, dbContext);

            return View();
        }
    }
}