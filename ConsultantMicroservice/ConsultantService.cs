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
        public List<Shared.ConsultantModel> GetConsultants()
        {
            return _dbContext.Consultant.ToList();
        }

    }
}
