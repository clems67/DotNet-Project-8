using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CalifornianHealthMonolithic.Models
{
    public class ConsultantModel
    {
        public int id { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string speciality { get; set; }
    }
        public class ConsultantModelv2
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Speciality { get; set; }
    }

    public class ConsultantModelList
    {
        public List<ConsultantCalendar> consultantCalendars { get; set; }
        public List<ConsultantModelv2> consultants { get; set; }
        public int selectedConsultantId { get; set; }
        public SelectList ConsultantsList { get; set; }
    }

}