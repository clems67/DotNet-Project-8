using System.Diagnostics;

namespace ConsultantMicroservice
{
    public class ConsultantService : IConsultantService
    {
        private readonly IConsultantDBContext _dbContext;
        public ConsultantService(IConsultantDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<Shared.ConsultantModel> GetConsultants()
        {
            return _dbContext.Consultant.ToList();
        }
    }
}
