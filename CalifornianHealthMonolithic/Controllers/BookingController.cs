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
        private RpcClient rpcClient = new RpcClient("Appointment_queue");

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
            var response = rpcClient.CallAsync(
                new CommunicationModel
                {
                    AccessTypeSelected = CommunicationModel.AccessType.getAppointments,
                    ConsultantIdSelected = id
                }
            );

            var timeoutCounter = 0;
            while (rpcClient.communicationModel == null)
            {
                timeoutCounter += 1;
                Thread.Sleep(100);
                if (timeoutCounter == 150)
                {
                    return JsonConvert.SerializeObject(new CommunicationModel { AccessTypeSelected = CommunicationModel.AccessType.overtime });
                }
            }
            return JsonConvert.SerializeObject(rpcClient.communicationModel);
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