using ConsultantMicroservice;

namespace AppointmentMicroservice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            MessageServiceSetup messageServiceSetup = new MessageServiceSetup(builder.Services.BuildServiceProvider());
            messageServiceSetup.Setup();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}
