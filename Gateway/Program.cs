
using Microsoft.Extensions.DependencyInjection;

namespace Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var appointmentRpcClient = new RpcClient("Appointment_queue");
            var consultantRpcClient = new RpcClient("Consultant_queue");
            builder.Services.AddSingleton<IAppointmentRpcClient>(appointmentRpcClient);
            builder.Services.AddSingleton<IConsultantRpcClient>(consultantRpcClient);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
