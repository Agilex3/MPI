using Catalogue.Models;
using Catalogue.Services;
using Microsoft.AspNetCore.Mvc;

namespace Catalogue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GradeApiController : ControllerBase
    {
        private readonly GradeService _gradeService;

        public GradeApiController(GradeService gradeService)
        {
            _gradeService = gradeService;
        }

        // GET: api/GradeApi
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var grades = await _gradeService.GetAllGradesAsync();
            return Ok(grades);
        }

        // GET: api/GradeApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var grade = await _gradeService.GetGradeByIdAsync(id);
            if (grade == null)
                return NotFound(new { message = "Grade not found" });

            return Ok(grade);
        }

        // POST: api/GradeApi
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Grade grade)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _gradeService.CreateGradeAsync(grade);
            return Ok(new { message = "Grade created successfully", created });
        }

        // PUT: api/GradeApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Grade grade)
        {
            var success = await _gradeService.UpdateGradeAsync(id, grade);
            if (!success)
                return NotFound(new { message = "Grade not found" });

            return Ok(new { message = "Grade updated successfully" });
        }

        // DELETE: api/GradeApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _gradeService.DeleteGradeAsync(id);
            if (!success)
                return NotFound(new { message = "Grade not found" });

            return Ok(new { message = "Grade deleted successfully" });
        }
    }
}
