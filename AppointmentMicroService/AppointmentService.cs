using Shared;
using System.Linq;

namespace AppointmentMicroservice
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppointmentDbContext _dbContext;
        public AppointmentService(AppointmentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool CreateAppointment(AppointmentModel appointment)
        {
            bool alreadyBooked = _dbContext.Appointment
                .Where(a => a.ConsultantId == appointment.ConsultantId)
                .Where(a => a.startDate == appointment.startDate)
                .Any();
            if (alreadyBooked) {
                return false;
            };
            appointment.endDate = appointment.startDate.AddMinutes(30);
            _dbContext.Appointment.Add(appointment);
            _dbContext.SaveChanges();
            return true;
        }

        public List<AppointmentModel> GetRecentAppointments(int id)
        {
            return _dbContext.Appointment
                .Where(a => a.ConsultantId == id)
                .Where(a => a.startDate > DateTime.Now)
                .ToList();
        }
    }
}
