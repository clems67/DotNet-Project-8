using CalifornianHealthMonolithic.Code;
using CalifornianHealthMonolithic.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = "guest";
            factory.Password = "guest";
            factory.HostName = "localhost";
            factory.Port = 5672;
            factory.ClientProvidedName = "Rabbit Sender Main App";

            IConnection conn = factory.CreateConnection();

            IModel channel = conn.CreateModel();

            string exchangeName = "CalifornianHealthExchange";
            string routingKey = "GET_APPOINTMENT";
            string queueName = "AppointmentQueue";

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey, null);

            byte[] messageBodyBytes = Encoding.UTF8.GetBytes("Hello World !");
            channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);

            channel.Close();
            conn.Close();

            return View(conList);
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