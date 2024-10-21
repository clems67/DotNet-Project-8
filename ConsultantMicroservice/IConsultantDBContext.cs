using Microsoft.EntityFrameworkCore;

namespace ConsultantMicroservice
{
    public interface IConsultantDBContext
    {
        public DbSet<ConsultantModel> Consultant { get; set; }
        public void SaveChanges();
    }
}
