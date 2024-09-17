using CalifornianHealthMonolithic.Code;
using CalifornianHealthMonolithic.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CalifornianHealthMonolithic.Controllers
{
    public class CreateAppointmentDto
    {
        public int consultantId { get; set; }
        public string patientName { get; set; }
    }
    public class BookingController : Controller
    {
        [System.Web.Mvc.HttpPost]
        public string TestPostMethod(CreateAppointmentDto createAppointmentDto)
        {
            Debug.WriteLine($"\nTestMehod consultantId: {createAppointmentDto.consultantId}\n");
            Debug.WriteLine("conflict");
            return "409";
        }
        [System.Web.Mvc.HttpGet]
        public string TestGetMethod(int id)
        {
            var returnedValue = new CreateAppointmentDto() { consultantId = 1, patientName = "bob" };
            return JsonConvert.SerializeObject(returnedValue);
        }
        public System.Web.Mvc.ActionResult GetConsultantCalendar()
        {
            //ConsultantModelList conList = new ConsultantModelList();
            //CHDBContext dbContext = new CHDBContext();
            //Repository repo = new Repository();
            //List<Consultant> cons = new List<Consultant>();
            //cons = repo.FetchConsultants(dbContext);
            //conList.ConsultantsList = new SelectList(cons, "Id", "FName");
            //conList.consultants = cons;

            return View(/*conList*/);
        }
            [System.Web.Mvc.HttpGet]
        public string GetConsultantCalendarRequest(int id)
        {
            var rpcClient = new RpcClient();
            var response = rpcClient.CallAsync(id);

            var timeoutCounter = 0;
            while(rpcClient.communicationModel == null)
            {
                timeoutCounter += 1;
                Thread.Sleep(100);
                if(timeoutCounter == 150)
                {
                    return JsonConvert.SerializeObject(new AppointmentCommunicationModel { AccessTypeSelected = AppointmentCommunicationModel.AccessType.overtime});
                }
            }
            return JsonConvert.SerializeObject(rpcClient.communicationModel);           
        }

        public class RpcClient : IDisposable
        {
            private readonly string exchangeName = string.Empty;
            private readonly string routingKey = "GET_APPOINTMENT";
            private const string QUEUE_NAME = "Appointment_queue";

            private IConnection connection;
            private IModel channel;
            private string replyQueueName;
            private ConcurrentDictionary<string, TaskCompletionSource<string>> callbackMapper = new ConcurrentDictionary<string, TaskCompletionSource<string>>();
            internal AppointmentCommunicationModel communicationModel;

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
                    var responseString = Encoding.UTF8.GetString(body);
                    communicationModel = JsonConvert.DeserializeObject<AppointmentCommunicationModel>(responseString);
                    tcs.TrySetResult(responseString);
                };

                channel.BasicConsume(consumer: consumer,
                                     queue: replyQueueName,
                                     autoAck: false);
            }

            public Task<string> CallAsync(int id, CancellationToken cancellationToken = default)
            {
                Debug.WriteLine("\nCallAsync\n");
                IBasicProperties props = channel.CreateBasicProperties();
                var correlationId = Guid.NewGuid().ToString();
                props.CorrelationId = correlationId;
                props.ReplyTo = replyQueueName;

                var communicationModel = new AppointmentCommunicationModel()
                {
                    AccessTypeSelected = AppointmentCommunicationModel.AccessType.getAppointments,
                    ConsultantIdSelected = id
                };
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

        //TODO: Change this method to ensure that members do not have to wait endlessly. 
        public System.Web.Mvc.ActionResult ConfirmAppointment(Appointment model)
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