using CalifornianHealthMonolithic.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CalifornianHealthMonolithic.Controllers
{
    public class BookingController : Controller
    {
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> GetConsultantCalendar()
        {
            HttpClient httpClient = new HttpClient();
            var responseContent = await (await httpClient.GetAsync("https://localhost:7092/Consultant")).Content.ReadAsStringAsync();
            var communicationModel = JsonConvert.DeserializeObject<List<ConsultantModelV2>>(responseContent);

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
    }
}