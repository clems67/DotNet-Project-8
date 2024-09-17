using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class AppointmentCommunicationModel
    {
        public enum AccessType { getAppointments, createNewAppointment, error, overtime }
        public AccessType AccessTypeSelected { get; set; }
        public int ConsultantIdSelected { get; set; }
        public List<AppointmentModel> Appointments { get; set; }
        public AppointmentModel AppointmentToCreate { get; set; }
        public bool IsAppointmentsCreated { get; set; }
    }
}
