using Microsoft.EntityFrameworkCore;

namespace AppointmentMicroservice
{
    public interface IAppointmentDbContext
    {
        public DbSet<AppointmentModel> Appointment { get; set; }
        public void SaveChanges ();
    }
}
