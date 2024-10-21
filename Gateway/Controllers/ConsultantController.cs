using Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConsultantController : ControllerBase
    {
        private IConsultantRpcClient _consultantRpcClient;
        public ConsultantController(IConsultantRpcClient consultantRpcClient)
        {
            _consultantRpcClient = consultantRpcClient;
        }

        [HttpGet]
        public async Task<List<ConsultantModel>> GetConsultantsList()
        {
            var responseString = await _consultantRpcClient.CallConsultantMicroserviceAsync(
                new ConsultantCommunicationModel
                {
                    AccessTypeSelected = ConsultantCommunicationModel.AccessType.getConsultants
                });
            var response = JsonConvert.DeserializeObject<ConsultantCommunicationModel>(responseString);
            return response.Consultants;
        }
    }
}
