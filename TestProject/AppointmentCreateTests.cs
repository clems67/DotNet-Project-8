using AppointmentMicroservice;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shared;

namespace TestProject
{
    public class AppointmentCreateTests
    {
        private readonly Mock<IAppointmentDbContext> _mockContext;
        private readonly AppointmentService _appointmentService;

        public AppointmentCreateTests()
        {
            _mockContext = new Mock<IAppointmentDbContext>();

            var appointments = new List<AppointmentModel>().AsQueryable();
            var mockSet = new Mock<DbSet<Shared.AppointmentModel>>();
            mockSet.As<IQueryable<AppointmentModel>>().Setup(m => m.Provider).Returns(appointments.Provider);
            mockSet.As<IQueryable<AppointmentModel>>().Setup(m => m.Expression).Returns(appointments.Expression);
            mockSet.As<IQueryable<AppointmentModel>>().Setup(m => m.ElementType).Returns(appointments.ElementType);
            mockSet.As<IQueryable<AppointmentModel>>().Setup(m => m.GetEnumerator()).Returns(appointments.GetEnumerator());
        
            _mockContext.Setup(c => c.Appointment).Returns(mockSet.Object);

            _appointmentService = new AppointmentService(_mockContext.Object);
        }

        [Fact]
        public void CreateAppointment_RoundsStartDateToNearestHour()
        {
            // Arrange
            var appointment = new AppointmentModel
            {
                ConsultantId = 1,
                startDate = DateTime.Parse("2024-10-01 13:05:11"),
                endDate = DateTime.MinValue
            };

            // Act
            bool result = _appointmentService.CreateAppointment(appointment);

            // Assert
            Assert.True(result);
            Assert.Equal(DateTime.Parse("2024-10-01 13:00:00"), appointment.startDate);
            Assert.Equal(DateTime.Parse("2024-10-01 13:30:00"), appointment.endDate);

            _mockContext.Verify(m => m.Appointment.Add(It.IsAny<AppointmentModel>()), Times.Once);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CreateAppointment_RoundsStartDateToNearestHalfHour()
        {
            // Arrange
            var appointment = new AppointmentModel
            {
                ConsultantId = 1,
                startDate = DateTime.Parse("2024-10-01 13:48:11"),
                endDate = DateTime.MinValue
            };

            // Act
            bool result = _appointmentService.CreateAppointment(appointment);

            // Assert
            Assert.True(result);
            Assert.Equal(DateTime.Parse("2024-10-01 13:30:00"), appointment.startDate);
            Assert.Equal(DateTime.Parse("2024-10-01 14:00:00"), appointment.endDate);

            _mockContext.Verify(m => m.Appointment.Add(It.IsAny<AppointmentModel>()), Times.Once);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }
    }
}
