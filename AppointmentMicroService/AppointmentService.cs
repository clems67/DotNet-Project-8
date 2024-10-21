using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AppointmentMicroservice
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentDbContext _dbContext;
        public AppointmentService(IAppointmentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool CreateAppointment(AppointmentModel appointment)
        {
            DateTime hoursDateTime = appointment.startDate.Date.AddHours(appointment.startDate.Hour);
            if (appointment.startDate.Minute >= 30)
            {
                appointment.startDate = hoursDateTime.AddMinutes(30);
            }
            else
            {
                appointment.startDate = hoursDateTime;
            }
            bool alreadyBooked = _dbContext.Appointment
                .Where(a => a.ConsultantId == appointment.ConsultantId)
                .Where(a => a.startDate == appointment.startDate)
                .Any();
            if (alreadyBooked)
            {
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
