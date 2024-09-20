using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using Shared;
using System.Net.Http.Json;

namespace AppointmentMicroservice
{
    public class MessageServiceSetup : Shared.MessageServiceSetup
    {
        public IAppointmentService AppointmentService { get; set; }
        public MessageServiceSetup(IServiceProvider serviceProvider) : base("Appointment_queue")
    {
            this.AppointmentService = serviceProvider.GetService<IAppointmentService>();
        }

        public override CommunicationModel MessageHandler(CommunicationModel questionModel)
        {
            if (questionModel.AccessTypeSelected == CommunicationModel.AccessType.getAppointments)
            {
                questionModel.Appointments = AppointmentService.GetRecentAppointments(questionModel.ConsultantIdSelected);
            }
            else if (questionModel.AccessTypeSelected == CommunicationModel.AccessType.createNewAppointment)
            {
                questionModel.IsAppointmentsCreated = AppointmentService.CreateAppointment(questionModel.AppointmentToCreate);                
            }
            else
            {
                questionModel.AccessTypeSelected = CommunicationModel.AccessType.error;
            }
            return questionModel;
        }
    }
}
