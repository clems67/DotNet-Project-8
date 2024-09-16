using Shared;

namespace AppointmentMicroservice
{
    public interface IAppointmentService
    {
        List<Shared.AppointmentModel> GetRecentAppointments();
        bool CreateAppointment(AppointmentModel appointment);
    }
}
