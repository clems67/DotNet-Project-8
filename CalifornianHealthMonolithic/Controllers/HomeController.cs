using CalifornianHealthMonolithic.Code;
using CalifornianHealthMonolithic.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CalifornianHealthMonolithic.Controllers
{

    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            HttpClient httpClient = new HttpClient();
            var responseContent = await (await httpClient.GetAsync("http://localhost:5250/Consultant")).Content.ReadAsStringAsync();
            var consultants = JsonConvert.DeserializeObject<List<ConsultantModelV2>>(responseContent);

           
                var response = new List<Models.ConsultantModel>();
                foreach (var consultant in consultants)
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