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
        public async Task<int> GetAppointment()
        {
            var result = _dbContext.Consultant.Count();
            Debug.WriteLine($"consultant service - get appointment - result: {result}");
            return result;
        }

    }
}
