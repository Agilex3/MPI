using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Catalogue.Data;
using Catalogue.Models;
using Catalogue.Services;


namespace Catalogue.Tests
{
    public class GradeServiceTests
    {
        [Fact]
        public async Task CalculateAverageGradeForStudentAsync_ReturnsCorrectAverage()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using var context = new MyDbContext(options);
            var service = new GradeService(context);

            context.Grades.AddRange(
                new Grade { StudentId = 1, GradeValue = 8 },
                new Grade { StudentId = 1, GradeValue = 10 },
                new Grade { StudentId = 1, GradeValue = 9 }
            );
            await context.SaveChangesAsync();

            // Act
            var result = await service.CalculateAvgGradeForStudent(1);

            // Assert
            Assert.Equal(9.00m, result);
        }

        [Fact]
        public async Task CalculateAverageGradeForStudentAsync_ReturnsNull_IfNoGrades()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase("EmptyDb")
                .Options;

            using var context = new MyDbContext(options);
            var service = new GradeService(context);

            var result = await service.CalculateAvgGradeForStudent(99);

            Assert.Null(result);
        }
    }
}
