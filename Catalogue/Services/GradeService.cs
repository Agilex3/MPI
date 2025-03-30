using Catalogue.Data;
using Catalogue.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalogue.Services
{
    public class GradeService
    {
        private readonly MyDbContext _context;

        public GradeService(MyDbContext context)
        {
            _context = context;
        }

        // Obține toate înregistrările de Grade, inclusiv datele studentului și cursului
        public async Task<List<Grade>> GetAllGradesAsync()
        {
            return await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Course)
                .ToListAsync();
        }

        // Obține un Grade după ID
        public async Task<Grade?> GetGradeByIdAsync(int id)
        {
            return await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Course)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        // Creează un nou Grade
        public async Task<Grade> CreateGradeAsync(Grade grade)
        {
            // Setează data evaluării la momentul creării
            grade.GradedAt = DateTime.Now;
            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();
            return grade;
        }

        // Actualizează un Grade existent
        public async Task<bool> UpdateGradeAsync(int id, Grade updatedGrade)
        {
            var existingGrade = await _context.Grades.FindAsync(id);
            if (existingGrade == null)
                return false;

            existingGrade.StudentId = updatedGrade.StudentId;
            existingGrade.CourseId = updatedGrade.CourseId;
            existingGrade.GradeValue = updatedGrade.GradeValue;
            // Poți decide dacă vrei să actualizezi și GradedAt sau să-l lași la valoarea inițială.
            existingGrade.GradedAt = updatedGrade.GradedAt;

            await _context.SaveChangesAsync();
            return true;
        }

        // Șterge un Grade
        public async Task<bool> DeleteGradeAsync(int id)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade == null)
                return false;

            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
