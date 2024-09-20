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
        public string GetConsultantList()
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
                    return JsonConvert.SerializeObject(new CommunicationModel { AccessTypeSelected = CommunicationModel.AccessType.overtime });
                }
            }
            return JsonConvert.SerializeObject(rpcClient.communicationModel);
        }
        public ActionResult Index()
        {
            return View(/*GetConsultantList()*/);
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