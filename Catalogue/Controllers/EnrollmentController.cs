using Catalogue.Models;
using Catalogue.Services;
using Microsoft.AspNetCore.Mvc;

namespace Catalogue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentApiController : ControllerBase
    {
        private readonly EnrollmentService _enrollmentService;

        public EnrollmentApiController(EnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        // GET: api/EnrollmentApi
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var enrollments = await _enrollmentService.GetAllAsync();
            return Ok(enrollments);
        }

        // GET: api/EnrollmentApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var enrollment = await _enrollmentService.GetByIdAsync(id);
            if (enrollment == null)
                return NotFound(new { message = "Enrollment not found" });

            return Ok(enrollment);
        }

        // POST: api/EnrollmentApi
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Enrollment enrollment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _enrollmentService.CreateAsync(enrollment);
            return Ok(new { message = "Enrollment created", created });
        }

        // PUT: api/EnrollmentApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Enrollment enrollment)
        {
            var success = await _enrollmentService.UpdateAsync(id, enrollment);
            if (!success)
                return NotFound(new { message = "Enrollment not found" });

            return Ok(new { message = "Enrollment updated" });
        }

        // DELETE: api/EnrollmentApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _enrollmentService.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = "Enrollment not found" });

            return Ok(new { message = "Enrollment deleted" });
        }
    }
}
