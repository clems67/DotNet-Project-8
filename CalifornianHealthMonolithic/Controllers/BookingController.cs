using CalifornianHealthMonolithic.Code;
using CalifornianHealthMonolithic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.Xml;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace CalifornianHealthMonolithic.Controllers
{
    public class BookingController : Controller
    {
        public RpcClient rpcClient;

        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> GetConsultantCalendar()
        {
            HttpClient httpClient = new HttpClient();
            var responseContent = await (await httpClient.GetAsync("http://localhost:55258/Home/GetConsultants")).Content.ReadAsStringAsync();
            var communicationModel = JsonConvert.DeserializeObject<List<Shared.ConsultantModel>>(responseContent);

            var consultants = new List<Models.ConsultantModel>();
            foreach (var consultant in communicationModel)
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
        public async Task<string> GetConsultantCalendarRequest(int id)
        {
            rpcClient.communicationModel = null;
            var response = await rpcClient.CallAsync(
                new CommunicationModel
                {
                    AccessTypeSelected = CommunicationModel.AccessType.getAppointments,
                    ConsultantIdSelected = id
                }
            );

            return JsonConvert.SerializeObject(rpcClient.communicationModel.Appointments);
        }

        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> BookAppointment([FromBody] AppointmentModel appointmentModel)
        {
            var communicationModel = new CommunicationModel
            {
                AccessTypeSelected = CommunicationModel.AccessType.createNewAppointment,
                AppointmentToCreate = appointmentModel
            };
            var response = await rpcClient.CallAsync(communicationModel);

            if (rpcClient != null && rpcClient.communicationModel != null)
            {
                int statusCode = rpcClient.communicationModel.IsAppointmentsCreated ? 200 : 409;
                return new HttpStatusCodeResult(statusCode);
            }
            return new HttpStatusCodeResult(500);
        }
    }
}