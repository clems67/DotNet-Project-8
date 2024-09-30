using Microsoft.EntityFrameworkCore;
using Shared;

namespace AppointmentMicroservice
{
    public interface IAppointmentDbContext
    {
        public DbSet<Shared.AppointmentModel> Appointment { get; set; }
        public void SaveChanges ();
    }
}
