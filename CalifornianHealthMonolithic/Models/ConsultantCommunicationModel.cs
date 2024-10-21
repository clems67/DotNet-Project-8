using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CalifornianHealthMonolithic.Models
{
    public class ConsultantCommunicationModel
    {
        public class CommunicationModel
        {
            public enum AccessType { noAccessTypeSelected, getAppointments, createNewAppointment, getConsultants, error, overtime }
            public AccessType AccessTypeSelected = AccessType.noAccessTypeSelected;
            public int ConsultantIdSelected { get; set; }
            public List<ConsultantModel> Consultants { get; set; }
        }
    }
}