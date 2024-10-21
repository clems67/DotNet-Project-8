namespace AppointmentMicroservice
{
    public interface IAppointmentService
    {
        List<AppointmentModel> GetRecentAppointments(int id);
        bool CreateAppointment(AppointmentModel appointment);
    }
}
