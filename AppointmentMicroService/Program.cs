using ConsultantMicroservice;
using Microsoft.EntityFrameworkCore;

namespace AppointmentMicroservice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            builder.Services.AddDbContext<AppointmentDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("WebApiDatabase")));

            var app = builder.Build();

            MessageServiceSetup messageServiceSetup = new MessageServiceSetup(builder.Services.BuildServiceProvider());
            messageServiceSetup.Setup();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}
