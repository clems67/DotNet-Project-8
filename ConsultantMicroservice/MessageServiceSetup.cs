using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using Shared;

namespace ConsultantMicroservice
{
    public class MessageServiceSetup : Shared.MessageServiceSetup
    {
        public IConsultantService ConsultantController { get; set; }
        public MessageServiceSetup(IServiceProvider serviceProvider) : base("Consultant_queue")
        {
            this.ConsultantController = serviceProvider.GetService<IConsultantService>();
        }

        public override CommunicationModel MessageHandler(CommunicationModel communicationModel)
        {
            var response = new CommunicationModel(){};
            if(communicationModel.AccessTypeSelected == CommunicationModel.AccessType.getConsultants)
            {
                response.Consultants = ConsultantController.GetConsultants();
                response.AccessTypeSelected = CommunicationModel.AccessType.getConsultants;
            }
            else
            {
                Debug.Write("\nERROR MessageHandler MessageService ConsultantMicroservice\n");
                response.AccessTypeSelected = CommunicationModel.AccessType.error;
            }
            return response;
        }
    }
}
