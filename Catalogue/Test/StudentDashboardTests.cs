using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Catalogue.Data;
using Catalogue.Models;
using Catalogue.Services;
using Microsoft.AspNetCore.Mvc;
using Catalogue.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace Catalogue.Tests
{
    public class StudentDashboardTests
    {
        private DbContextOptions<MyDbContext> GetOptions(string databaseName)
        {
            return new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;
        }

        [Fact]
        public async Task GetAverageForStudent_ReturnsCorrectAveragesPerCourse()
        {
            // Arrange
            using var context = new MyDbContext(GetOptions("StudentDashboardTestDb1"));

            // Seed data
            var student = new User
            {
                id = 1,
                email = "student@test.com",
                first_name = "John",
                last_name = "Doe",
                role = "Student",
                created_at = DateTime.UtcNow
            };
            student.SetPassword("Password123");

            var teacher = new User
            {
                id = 2,
                email = "teacher@test.com",
                first_name = "Jane",
                last_name = "Smith",
                role = "Teacher",
                created_at = DateTime.UtcNow
            };
            teacher.SetPassword("Password123");

            var course1 = new Course
            {
                Id = 1,
                Name = "Math",
                Description = "Mathematics",
                TeacherId = 2,
                Teacher = teacher,
                CreatedAt = DateTime.UtcNow
            };
            var course2 = new Course
            {
                Id = 2,
                Name = "Physics",
                Description = "Physics",
                TeacherId = 2,
                Teacher = teacher,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.AddRange(student, teacher);
            context.Courses.AddRange(course1, course2);

            context.Grades.AddRange(
                new Grade { StudentId = 1, Student = student, CourseId = 1, Course = course1, GradeValue = 8, GradedAt = DateTime.UtcNow },
                new Grade { StudentId = 1, Student = student, CourseId = 1, Course = course1, GradeValue = 10, GradedAt = DateTime.UtcNow },
                new Grade { StudentId = 1, Student = student, CourseId = 2, Course = course2, GradeValue = 7, GradedAt = DateTime.UtcNow },
                new Grade { StudentId = 1, Student = student, CourseId = 2, Course = course2, GradeValue = 9, GradedAt = DateTime.UtcNow }
            );

            await context.SaveChangesAsync();

            var gradeService = new GradeService(context);
            var controller = new GradeApiController(gradeService);

            // Act
            var result = await controller.GetAverageForStudent(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<List<object>>(okResult.Value);

            Assert.Equal(2, returnValue.Count);

            var mathAverage = returnValue.First(x => ((dynamic)x).Course == "Math");
            Assert.Equal(9.00m, ((dynamic)mathAverage).Average);

            var physicsAverage = returnValue.First(x => ((dynamic)x).Course == "Physics");
            Assert.Equal(8.00m, ((dynamic)physicsAverage).Average);
        }

        [Fact]
        public async Task GetAverageForStudent_NoGrades_ReturnsEmptyMessage()
        {
            // Arrange
            using var context = new MyDbContext(GetOptions("StudentDashboardTestDb2"));

            var student = new User
            {
                id = 1,
                email = "student@test.com",
                first_name = "John",
                last_name = "Doe",
                role = "Student",
                created_at = DateTime.UtcNow
            };
            student.SetPassword("Password123");

            context.Users.Add(student);
            await context.SaveChangesAsync();

            var gradeService = new GradeService(context);
            var controller = new GradeApiController(gradeService);

            // Act
            var result = await controller.GetAverageForStudent(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = okResult.Value;
            Assert.NotNull(returnValue);
            Assert.Equal("No averages found.", ((dynamic)returnValue).message);
        }
    }
}