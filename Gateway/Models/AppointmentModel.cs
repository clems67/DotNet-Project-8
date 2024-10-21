using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gateway.Models
{
    public class AppointmentModel
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public int ConsultantId { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }
}