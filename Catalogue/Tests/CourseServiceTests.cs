using Catalogue.Models;
using Catalogue.Services;
using Catalogue.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalogue.Tests
{
    public class CourseServiceTests
    {
        private DbContextOptions<MyDbContext> _dbOptions;

        public CourseServiceTests()
        {
            _dbOptions = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetAllCoursesAsync_ReturnsAllCourses()
        {
            using var context = new MyDbContext(_dbOptions);

            var teacher1 = new User { id = 1, first_name = "Alan", last_name = "Turing", email = "alan@example.com", password = "123", role = "teacher", created_at = DateTime.Now };
            var teacher2 = new User { id = 2, first_name = "Isaac", last_name = "Newton", email = "isaac@example.com", password = "123", role = "teacher", created_at = DateTime.Now };

            context.Users.AddRange(teacher1, teacher2);

            context.Courses.AddRange(
                new Course { Name = "Math", Description = "Algebra", TeacherId = 1 },
                new Course { Name = "Physics", Description = "Mechanics", TeacherId = 2 });

            await context.SaveChangesAsync();

            var service = new CourseService(context);
            var result = await service.GetAllCoursesAsync();

            Assert.Equal(2, result.Count);
        }


        [Fact]
        public async Task GetCourseByIdAsync_ReturnsCorrectCourse()
        {
            using var context = new MyDbContext(_dbOptions);

            // Add a teacher (User) first
            var teacher = new User { id = 3, first_name = "Prof." ,last_name="oak",email="cv@cv.ro", password="bla",role="teacher",created_at=new DateTime()};
            context.Users.Add(teacher);

            // Then add the course that references that teacher
            var course = new Course
            {
                Name = "Biology",
                Description = "Cells",
                TeacherId = teacher.id
            };
            context.Courses.Add(course);

            await context.SaveChangesAsync();

            var service = new CourseService(context);
            var result = await service.GetCourseByIdAsync(course.Id);

            Assert.NotNull(result);
            Assert.Equal("Biology", result.Name);
            Assert.NotNull(result.Teacher); // Also validates the include worked
            Assert.Equal("Prof.", result.Teacher.first_name);
        }



        [Fact]
        public async Task CreateCourseAsync_AddsNewCourse()
        {
            using var context = new MyDbContext(_dbOptions);
            var service = new CourseService(context);

            var newCourse = new Course { Name = "History", Description = "Ancient", TeacherId = 4 };
            var result = await service.CreateCourseAsync(newCourse);

            Assert.NotEqual(0, result.Id);
            Assert.Single(await context.Courses.ToListAsync());
        }

        [Fact]
        public async Task UpdateCourseAsync_UpdatesCourse()
        {
            using var context = new MyDbContext(_dbOptions);
            var course = new Course { Name = "Chemistry", Description = "Organic", TeacherId = 5 };
            context.Courses.Add(course);
            await context.SaveChangesAsync();

            var service = new CourseService(context);
            var updatedCourse = new Course { Name = "Advanced Chemistry", Description = "Inorganic", TeacherId = 6 };

            var success = await service.UpdateCourseAsync(course.Id, updatedCourse);

            Assert.True(success);
            var result = await context.Courses.FindAsync(course.Id);
            Assert.Equal("Advanced Chemistry", result.Name);
        }

        [Fact]
        public async Task DeleteCourseAsync_RemovesCourse()
        {
            using var context = new MyDbContext(_dbOptions);
            var course = new Course { Name = "Geography", Description = "Maps", TeacherId = 7 };
            context.Courses.Add(course);
            await context.SaveChangesAsync();

            var service = new CourseService(context);
            var result = await service.DeleteCourseAsync(course.Id);

            Assert.True(result);
            Assert.Empty(await context.Courses.ToListAsync());
        }

        [Fact]
        public async Task DeleteCourseAsync_ReturnsFalse_IfCourseNotFound()
        {
            using var context = new MyDbContext(_dbOptions);
            var service = new CourseService(context);

            var result = await service.DeleteCourseAsync(999); // Non-existent ID

            Assert.False(result);
        }
    }
}
