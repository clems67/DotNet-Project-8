using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class AppointmentCommunicationModel
    {
        public enum AccessType { getAppointments, createNewAppointment, error }
        public AccessType AccessTypeSelected { get; set; }
        public List<AppointmentModel> Appointments { get; set; }
        public AppointmentModel AppointmentToCreate { get; set; }
        public bool IsAppointmentsCreated { get; set; }
    }
}
