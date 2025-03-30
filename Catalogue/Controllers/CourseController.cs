using Catalogue.Models;
using Catalogue.Services;
using Microsoft.AspNetCore.Mvc;

namespace Catalogue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseApiController : ControllerBase
    {
        private readonly CourseService _courseService;

        public CourseApiController(CourseService courseService)
        {
            _courseService = courseService;
        }

        // GET: api/CourseApi
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        // GET: api/CourseApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound(new { message = "Course not found" });

            return Ok(course);
        }

        // POST: api/CourseApi
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Course course)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _courseService.CreateCourseAsync(course);
            return Ok(new { message = "Course created successfully", created });
        }

        // PUT: api/CourseApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Course course)
        {
            var success = await _courseService.UpdateCourseAsync(id, course);
            if (!success)
                return NotFound(new { message = "Course not found" });

            return Ok(new { message = "Course updated successfully" });
        }

        // DELETE: api/CourseApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _courseService.DeleteCourseAsync(id);
            if (!success)
                return NotFound(new { message = "Course not found" });

            return Ok(new { message = "Course deleted successfully" });
        }
    }
}
