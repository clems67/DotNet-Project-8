using CalifornianHealthMonolithic.Code;
using CalifornianHealthMonolithic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CalifornianHealthMonolithic.Controllers
{
    
    public class HomeController : Controller
    {
        public ActionResult Index()
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