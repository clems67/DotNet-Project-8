using Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppointmentController : ControllerBase
    {
        private IAppointmentRpcClient _appointmentRpcClient;
        public AppointmentController(IAppointmentRpcClient appointmentRpcClient)
        {
            _appointmentRpcClient = appointmentRpcClient;
        }

        [HttpGet]
        public async Task<List<AppointmentModel>> GetConsultantCalendarRequest(int consultantId)
        {
            var responseString = await _appointmentRpcClient.CallAppointmentMicroserviceAsync(
                    new AppointmentCommunicationModel
                    {
                        AccessTypeSelected = AppointmentCommunicationModel.AccessType.getAppointments,
                        ConsultantIdSelected = consultantId
                    }
                );
            var response = JsonConvert.DeserializeObject<AppointmentCommunicationModel>(responseString);

            return response.Appointments;
        }

        [HttpPost]
        public async Task<ActionResult> BookAppointment([FromBody] AppointmentModel appointmentModel)
        {
            if (appointmentModel.startDate < DateTime.Now)
            {
                return BadRequest();
            }
            var communicationModel = new AppointmentCommunicationModel
            {
                AccessTypeSelected = AppointmentCommunicationModel.AccessType.createNewAppointment,
                AppointmentToCreate = appointmentModel
            };
            var responseString = await _appointmentRpcClient.CallAppointmentMicroserviceAsync(communicationModel);

            var response = JsonConvert.DeserializeObject<AppointmentCommunicationModel>(responseString);

            if (response == null
                || response.AccessTypeSelected == AppointmentCommunicationModel.AccessType.error
                || response.AccessTypeSelected == AppointmentCommunicationModel.AccessType.overtime)
            {
                return Problem();
            }

            if (response.IsAppointmentsCreated)
            {
                return Ok();
            }
            else
            {
                return Conflict();
            }
        }
    }
}
