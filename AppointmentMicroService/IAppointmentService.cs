using Shared;

namespace AppointmentMicroservice
{
    public interface IAppointmentService
    {
        List<Shared.AppointmentModel> GetRecentAppointments(int id);
        bool CreateAppointment(AppointmentModel appointment);
    }
}
