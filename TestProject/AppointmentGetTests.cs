using AppointmentMicroservice;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shared;
using System.Net.Sockets;

namespace TestProject
{
    public class AppointmentGetTests : IDisposable
    {
        private Mock<IAppointmentDbContext> CreateMockContext(List<AppointmentModel> appointments)
        {
            var appointmentsQuerable = appointments.AsQueryable();
            var mockSet = new Mock<DbSet<AppointmentModel>>();
            mockSet.As<IQueryable<AppointmentModel>>().Setup(m => m.Provider).Returns(appointmentsQuerable.Provider);
            mockSet.As<IQueryable<AppointmentModel>>().Setup(m => m.Expression).Returns(appointmentsQuerable.Expression);
            mockSet.As<IQueryable<AppointmentModel>>().Setup(m => m.ElementType).Returns(appointmentsQuerable.ElementType);
            mockSet.As<IQueryable<AppointmentModel>>().Setup(m => m.GetEnumerator()).Returns(appointmentsQuerable.GetEnumerator());

            var mockContext = new Mock<IAppointmentDbContext>();
            mockContext.Setup(a => a.Appointment).Returns(mockSet.Object);

            return mockContext;
        }

        [Fact]
        public void GetAppointments_ReturnsEmptyList_WhenAppointmentsEarlierThanToday()
        {
            // Arrange
            var mockSet = new Mock<DbSet<AppointmentModel>>();

            var appointments = new List<AppointmentModel>
            {
                new AppointmentModel
                {
                    Id = 1,
                    ConsultantId = 1,
                    PatientName = "Test",
                    startDate = DateTime.Now.AddDays(-1),
                    endDate = DateTime.Now.AddDays(-1).AddMinutes(30),
                }
            };

            var mockContext = CreateMockContext(appointments);

            var appointmentService = new AppointmentService(mockContext.Object);

            // Act
            var result = appointmentService.GetRecentAppointments(1);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetAppointments_ReturnsList_WhenAppointmentsLaterThanToday()
        {
            // Arrange
            var appointments = new List<AppointmentModel>
            {
                new AppointmentModel
                {
                    Id = 1,
                    ConsultantId = 1,
                    PatientName = "Test",
                    startDate = DateTime.Now.AddDays(1),
                    endDate = DateTime.Now.AddDays(1).AddMinutes(30),
                }
            };

           var mockContext = CreateMockContext(appointments);
           
            var appointmentService = new AppointmentService(mockContext.Object);

            // Act
            var result = appointmentService.GetRecentAppointments(1);

            // Assert
            Assert.Single(result);
        }

        public void Dispose()
        {
            // Cleanup mock DbContext if needed
        }
    }
}