using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AppointmentMicroservice
{
    public class AppointmentDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public AppointmentDbContext(DbContextOptions<AppointmentDbContext> dbContextOptions, IConfiguration configuration) : base(dbContextOptions)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            //options.UseSqlServer(_configuration.GetConnectionString("WebApiDatabase"));
            //options.UseSqlServer("Server=localhost\\MSSQLLocalDB;Database=AppointmentDB;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true");
            options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AppointmentDB;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
        public DbSet<Shared.AppointmentModel> Consultant { get; set; }
    }
}
