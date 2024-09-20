﻿using Shared;
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
            if (_dbContext.Appointment.Any(a => a.startDate == appointment.startDate)) {
                return false;
            };
            _dbContext.Appointment.Add(appointment);
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
