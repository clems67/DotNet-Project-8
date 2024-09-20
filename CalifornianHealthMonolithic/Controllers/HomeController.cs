using CalifornianHealthMonolithic.Code;
using CalifornianHealthMonolithic.Models;
using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace CalifornianHealthMonolithic.Controllers
{

    public class HomeController : Controller
    {
        private RpcClient rpcClient = new RpcClient("Consultant_queue");

        [System.Web.Mvc.HttpGet]
        public CommunicationModel GetConsultantList()
        {
            var response = rpcClient.CallAsync(
                new CommunicationModel
                {
                    AccessTypeSelected = CommunicationModel.AccessType.getConsultants
                });
            var timeoutCounter = 0;
            while (rpcClient.communicationModel == null)
            {
                timeoutCounter += 1;
                Thread.Sleep(100);
                if (timeoutCounter == 150)
                {
                    return new CommunicationModel { AccessTypeSelected = CommunicationModel.AccessType.overtime };
                }
            }
            return rpcClient.communicationModel;
        }
        public ActionResult Index()
        {
            var value = GetConsultantList();
            if (value.AccessTypeSelected == CommunicationModel.AccessType.getConsultants)
            {
                var response = new List<Models.ConsultantModel>();
                foreach (var consultant in value.Consultants)
                {
                    response.Add(new Models.ConsultantModel()
                    {
                        id = consultant.Id,
                        fname = consultant.FirstName,
                        lname = consultant.LastName,
                        speciality = consultant.Speciality,
                    });
                }
                return View(new ConsultantModelList() { consultants = response });
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}