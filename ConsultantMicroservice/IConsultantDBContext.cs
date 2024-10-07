using Microsoft.EntityFrameworkCore;
using Shared;

namespace ConsultantMicroservice
{
    public interface IConsultantDBContext
    {
        public DbSet<Shared.ConsultantModel> Consultant { get; set; }
        public void SaveChanges();
    }
}
