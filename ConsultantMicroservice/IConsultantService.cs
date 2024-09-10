namespace ConsultantMicroservice
{
    public interface IConsultantService
    {
        Task<List<Shared.ConsultantModel>> GetAppointment();
    }
}
