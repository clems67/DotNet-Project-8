namespace AppointmentMicroService.Interfaces
{
    public interface IAppointmentService
    {
        Task<int> GetAppointment();
    }
}
