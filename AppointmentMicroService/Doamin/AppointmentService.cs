using AppointmentMicroService.Interfaces;
using AppointmentMicroService.Model;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Runtime.CompilerServices;
using System.Text;

namespace AppointmentMicroService.Controllers
{
    public class AppointmentService :IAppointmentService
    {
        private readonly ConsultantDBContext _dbContext;
        public AppointmentService(ConsultantDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> GetAppointment()
        {
            return _dbContext.Consultant.Count();
        }



    }
}
