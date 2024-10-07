using Microsoft.EntityFrameworkCore;
using Moq;
using Shared;
using ConsultantMicroservice;
using AppointmentMicroservice;

namespace TestProject
{
    public class ConsultantGetTests : IDisposable
    {
        private Mock<IConsultantDBContext> CreateMockContext(List<ConsultantModel> appointments)
        {
            var consultantsQuerable = appointments.AsQueryable();
            var mockSet = new Mock<DbSet<ConsultantModel>>();
            mockSet.As<IQueryable<ConsultantModel>>().Setup(m => m.Provider).Returns(consultantsQuerable.Provider);
            mockSet.As<IQueryable<ConsultantModel>>().Setup(m => m.Expression).Returns(consultantsQuerable.Expression);
            mockSet.As<IQueryable<ConsultantModel>>().Setup(m => m.ElementType).Returns(consultantsQuerable.ElementType);
            mockSet.As<IQueryable<ConsultantModel>>().Setup(m => m.GetEnumerator()).Returns(consultantsQuerable.GetEnumerator());

            var mockContext = new Mock<IConsultantDBContext>();
            mockContext.Setup(a => a.Consultant).Returns(mockSet.Object);

            return mockContext;
        }

        [Fact]
        public void GetConsultants_ReturnsEmptyList_WhenNoConsultantsInDatabase()
        {
            // Arrange
            var mockContext = CreateMockContext(new List<ConsultantModel>());

            var consultantservice = new ConsultantService(mockContext.Object);

            // Act
            var result = consultantservice.GetConsultants();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetConsultants_ReturnsAllConsultants_WhenConsultantsInDatabase()
        {
            // Arrange
            var consultants = new List<ConsultantModel>
                {
                    new ConsultantModel { Id = 1, FirstName = "John", LastName = "Doe", Speciality = "Doctor" },
                    new ConsultantModel { Id = 2, FirstName = "Jane", LastName = "Smith", Speciality = "Assistant" }
                };

            var mockContext = CreateMockContext(consultants);

            var consultantservice = new ConsultantService(mockContext.Object);

            // Act
            var result = consultantservice.GetConsultants();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Id == 1 && c.FirstName == "John" && c.LastName == "Doe");
            Assert.Contains(result, c => c.Id == 2 && c.FirstName == "Jane" && c.LastName == "Smith");
        }

        public void Dispose()
        {
            // Cleanup mock DbContext if needed
        }
    }
}