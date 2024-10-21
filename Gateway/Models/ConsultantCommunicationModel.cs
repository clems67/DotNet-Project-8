using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gateway.Models
{
    public class ConsultantCommunicationModel
    {
        public enum AccessType { noAccessTypeSelected, getAppointments, createNewAppointment, getConsultants, error, overtime }
        public AccessType AccessTypeSelected = AccessType.noAccessTypeSelected;
        public int ConsultantIdSelected { get; set; }
        public List<ConsultantModel> Consultants { get; set; }
    }
}