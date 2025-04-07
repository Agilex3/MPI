using Catalogue.Models;
using Catalogue.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

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
        public async Task<IActionResult> Create([FromBody] GradeDTO gradeDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

        var grade = new Grade
            {
            StudentId = gradeDTO.StudentId,
            //Student = gradeDTO.Student,
            CourseId = gradeDTO.CourseId,
            //Course = gradeDTO.Course,
            GradeValue = gradeDTO.GradeValue,
            GradedAt = gradeDTO.GradedAt,
            };

            // Ensure CourseId is set to 0 for auto-generation
            grade.Id = 0;
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

        // GET: api/GradeApi/average/5
        [HttpGet("average/{studentId}")]
        public async Task<IActionResult> GetAverageGrade(int studentId)
        {
            var average = await _gradeService.CalculateAvgGradeForStudent(studentId);

            if (average == null)
                return Ok(new { message = "No grades available for this student." });

            return Ok(new { studentId, averageGrade = average });
        }

        // Pentru profesor - vezi toate mediile pe materii, pe fiecare student
        [HttpGet("averages")]
        public async Task<IActionResult> GetAllAverages()
        {
            var averages = await _gradeService.GetAllStudentAveragesByCourse();
            return Ok(averages);
        }

        // Pentru student - doar media lui
        [HttpGet("average/student/{studentId}")]
        public async Task<IActionResult> GetAverageForStudent(int studentId)
        {
            var result = await _gradeService.GetStudentAveragesByCourse(studentId);

            if (result == null || !result.Any())
                return Ok(new { message = "No averages found." });

            return Ok(result);
        }



    }
}
