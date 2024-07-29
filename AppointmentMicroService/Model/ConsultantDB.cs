using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace AppointmentMicroService.Model
{
    public class ConsultantDBContext : DbContext
    {
        public ConsultantDBContext(DbContextOptions<ConsultantDBContext> dbContextOptions) : base (dbContextOptions)
        {           
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=localhost;Database=ConsultantDB;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true");
        }
        public DbSet<ConsultantTable> ConsultantDb { get; set; }

    }
}
