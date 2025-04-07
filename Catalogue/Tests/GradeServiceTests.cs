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

    private readonly DbContextOptions<MyDbContext> _options;

    public GradeServiceTests()
    {
        _options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task CreateGradeAsync_AddsGradeSuccessfully()
    {
        using var context = new MyDbContext(_options);
        var service = new GradeService(context);

        var student = new User { id = 1, first_name = "John", last_name = "Doe", email = "john@example.com", password = "123", role = "student", created_at = DateTime.Now };
        var course = new Course { Id = 1, Name = "Math", Description = "Algebra", TeacherId = 99 };

        context.Users.Add(student);
        context.Courses.Add(course);
        await context.SaveChangesAsync();

        var grade = new Grade
        {
            StudentId = student.id,
            CourseId = course.Id,
            GradeValue = 8.5m,
            GradedAt = DateTime.Now
        };

        var created = await service.CreateGradeAsync(grade);

        Assert.NotEqual(0, created.Id);
        Assert.Single(context.Grades);
    }

    [Fact]
    public async Task GetGradeByIdAsync_ReturnsCorrectGrade()
    {
        using var context = new MyDbContext(_options);
        var service = new GradeService(context);

        var student = new User { id = 2, first_name = "Jane", last_name = "Smith", email = "jane@example.com", password = "123", role = "student", created_at = DateTime.Now };
        var course = new Course { Id = 2, Name = "Science", Description = "Biology", TeacherId = 88 };

        context.Users.Add(student);
        context.Courses.Add(course);

        var grade = new Grade
        {
            StudentId = student.id,
            CourseId = course.Id,
            GradeValue = 9.0m,
            GradedAt = DateTime.Now
        };

        context.Grades.Add(grade);
        await context.SaveChangesAsync();

        var result = await service.GetGradeByIdAsync(grade.Id);

        Assert.NotNull(result);
        Assert.Equal(9.0m, result.GradeValue);
        Assert.Equal("Jane", result.Student.first_name);
        Assert.Equal("Science", result.Course.Name);
    }

    [Fact]
    public async Task CalculateAvgGradeForStudent_ReturnsCorrectAverage()
    {
        using var context = new MyDbContext(_options);
        var service = new GradeService(context);

        var student = new User { id = 3, first_name = "Ana", last_name = "Pop", email = "ana@example.com", password = "123", role = "student", created_at = DateTime.Now };
        var course = new Course { Id = 3, Name = "History", Description = "World History", TeacherId = 77 };

        context.Users.Add(student);
        context.Courses.Add(course);

        context.Grades.AddRange(
            new Grade { StudentId = student.id, CourseId = course.Id, GradeValue = 8.0m, GradedAt = DateTime.Now },
            new Grade { StudentId = student.id, CourseId = course.Id, GradeValue = 6.0m, GradedAt = DateTime.Now }
        );

        await context.SaveChangesAsync();

        var average = await service.CalculateAvgGradeForStudent(student.id);
        Assert.Equal(7.0m, average);
    }

    [Fact]
    public async Task GetAllGradesAsync_ReturnsAllGradesWithIncludes()
    {
        using var context = new MyDbContext(_options);
        var service = new GradeService(context);

        var student = new User { id = 4, first_name = "Mihai", last_name = "Ionescu", email = "mihai@example.com", password = "123", role = "student", created_at = DateTime.Now };
        var course = new Course { Id = 4, Name = "Geography", Description = "Maps", TeacherId = 66 };

        context.Users.Add(student);
        context.Courses.Add(course);

        context.Grades.Add(new Grade
        {
            StudentId = student.id,
            CourseId = course.Id,
            GradeValue = 10.0m,
            GradedAt = DateTime.Now
        });

        await context.SaveChangesAsync();

        var grades = await service.GetAllGradesAsync();
        Assert.Single(grades);
        Assert.NotNull(grades[0].Student);
        Assert.NotNull(grades[0].Course);
    }

    [Fact]
    public async Task UpdateGradeAsync_UpdatesDataCorrectly()
    {
        using var context = new MyDbContext(_options);
        var service = new GradeService(context);

        var student = new User { id = 5, first_name = "Elena", last_name = "Marin", email = "elena@example.com", password = "123", role = "student", created_at = DateTime.Now };
        var course = new Course { Id = 5, Name = "Physics", Description = "Mechanics", TeacherId = 55 };

        context.Users.Add(student);
        context.Courses.Add(course);

        var grade = new Grade
        {
            StudentId = student.id,
            CourseId = course.Id,
            GradeValue = 7.0m,
            GradedAt = DateTime.Now
        };
        context.Grades.Add(grade);
        await context.SaveChangesAsync();

        var updatedGrade = new Grade
        {
            StudentId = student.id,
            CourseId = course.Id,
            GradeValue = 9.0m,
            GradedAt = DateTime.Now
        };

        var result = await service.UpdateGradeAsync(grade.Id, updatedGrade);
        Assert.True(result);

        var modified = await context.Grades.FindAsync(grade.Id);
        Assert.Equal(9.0m, modified.GradeValue);
    }

    [Fact]
    public async Task DeleteGradeAsync_RemovesGrade()
    {
        using var context = new MyDbContext(_options);
        var service = new GradeService(context);

        var student = new User { id = 6, first_name = "Alex", last_name = "Popescu", email = "alex@example.com", password = "123", role = "student", created_at = DateTime.Now };
        var course = new Course { Id = 6, Name = "Art", Description = "Painting", TeacherId = 44 };

        context.Users.Add(student);
        context.Courses.Add(course);

        var grade = new Grade
        {
            StudentId = student.id,
            CourseId = course.Id,
            GradeValue = 5.5m,
            GradedAt = DateTime.Now
        };
        context.Grades.Add(grade);
        await context.SaveChangesAsync();

        var deleted = await service.DeleteGradeAsync(grade.Id);
        Assert.True(deleted);
        Assert.Empty(context.Grades);
    }
}
}

    