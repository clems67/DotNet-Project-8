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
using System.Net.Http;
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
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> GetConsultantCalendar()
        {
            //var response = new ConsultantModelList();
            //var consultants = new List<Models.ConsultantModel>();
            //consultants.Add(new Models.ConsultantModel { id = 1, fname = "Jessica" });
            //consultants.Add(new Models.ConsultantModel { id = 2, fname = "lai" });
            //consultants.Add(new Models.ConsultantModel { id = 3, fname = "Amanda" });
            //consultants.Add(new Models.ConsultantModel { id = 4, fname = "Jason" });
            //response.ConsultantsList = new SelectList(consultants, "Id", "fname");

            HttpClient httpClient = new HttpClient();
            var responseContent = await (await httpClient.GetAsync("http://localhost:55258/Home/GetConsultants")).Content.ReadAsStringAsync();
            var fagr= "";
            var communicationModel = JsonConvert.DeserializeObject<CommunicationModel>(responseContent);

            var consultants = new List<Models.ConsultantModel>();
            foreach (var consultant in communicationModel.Consultants)
            {
                consultants.Add(new Models.ConsultantModel
                {
                    id = consultant.Id,
                    fname = consultant.FirstName,
                    lname = consultant.LastName,
                    speciality = consultant.Speciality,
                });
            }
            var consultantModelList = new ConsultantModelList()
            {
                consultants = consultants,
                ConsultantsList = new SelectList(consultants, "Id", "FName")
            };
            return View(consultantModelList);
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