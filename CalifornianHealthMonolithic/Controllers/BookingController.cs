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
using System.Web.Http;
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
            HttpClient httpClient = new HttpClient();
            var responseContent = await (await httpClient.GetAsync("http://localhost:55258/Home/GetConsultants")).Content.ReadAsStringAsync();
            var fagr = "";
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
                    return "503"; //Service Unavailable error
                }
            }
            return JsonConvert.SerializeObject(rpcClient.communicationModel);
        }

        [System.Web.Mvc.HttpPost]
        public string BookAppointment([FromBody] AppointmentModel appointmentModel)
        {
            //CommunicationModel communicationModel = new CommunicationModel
            //{
            //    AccessTypeSelected = CommunicationModel.AccessType.createNewAppointment,
            //    AppointmentToCreate = new AppointmentModel
            //    {
            //        PatientName = PatientName,
            //        ConsultantId = ConsultantId,
            //        startDate = startDate
            //        //endDate = dateTime + TimeSpan.FromMinutes(30)
            //    }
            //};
            var communicationModel = new CommunicationModel
            {
                AccessTypeSelected = CommunicationModel.AccessType.createNewAppointment,
                AppointmentToCreate = appointmentModel
            };
            var response = rpcClient.CallAsync(communicationModel);

            var timeoutCounter = 0;
            while (rpcClient.communicationModel == null)
            {
                timeoutCounter += 1;
                Thread.Sleep(100);
                if (timeoutCounter == 150)
                {
                    return "503"; //Service Unavailable error
                }
            }
            if (rpcClient != null && rpcClient.communicationModel != null)
            {
                return rpcClient.communicationModel.IsAppointmentsCreated ? "200" : "409"; //OK or Conflict
            }
            return "500";
        }
    }
}