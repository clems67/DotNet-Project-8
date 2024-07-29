
using AppointmentMicroService.Controllers;
using AppointmentMicroService.Interfaces;
using AppointmentMicroService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmentMicroService
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

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IAppointmentService, AppointmentService>();

            builder.Services.AddDbContext<ConsultantDBContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("WebApiDatabase")));

            var app = builder.Build();

            MessageServiceSetup messageServiceSetup = new MessageServiceSetup(builder.Services.BuildServiceProvider());
            messageServiceSetup.Setup();



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
