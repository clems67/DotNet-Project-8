using System.Diagnostics;

namespace ConsultantMicroservice
{
    public class ConsultantService : IConsultantService
    {
        private readonly ConsultantDBContext _dbContext;
        public ConsultantService(ConsultantDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Shared.ConsultantModel>> GetAppointment()
        {
            return _dbContext.Consultant.ToList();
        }

    }
}
