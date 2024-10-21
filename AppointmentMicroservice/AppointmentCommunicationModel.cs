namespace AppointmentMicroservice
{
    public class AppointmentCommunicationModel
    {
            public enum AccessType { noAccessTypeSelected, getAppointments, createNewAppointment, error, overtime }
            public AccessType AccessTypeSelected = AccessType.noAccessTypeSelected;
            public int ConsultantIdSelected { get; set; }
            public List<AppointmentModel> Appointments { get; set; }
            public AppointmentModel AppointmentToCreate { get; set; }
            public bool IsAppointmentsCreated { get; set; }
    }
}
