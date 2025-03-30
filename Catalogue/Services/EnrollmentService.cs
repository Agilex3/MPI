using Catalogue.Data;
using Catalogue.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalogue.Services
{
    public class EnrollmentService
    {
        private readonly MyDbContext _context;

        public EnrollmentService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Enrollment>> GetAllAsync()
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();
        }

        public async Task<Enrollment?> GetByIdAsync(int id)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Enrollment> CreateAsync(Enrollment enrollment)
        {
            enrollment.EnrolledAt = DateTime.Now;
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task<bool> UpdateAsync(int id, Enrollment updatedEnrollment)
        {
            var existing = await _context.Enrollments.FindAsync(id);
            if (existing == null)
                return false;

            existing.StudentId = updatedEnrollment.StudentId;
            existing.CourseId = updatedEnrollment.CourseId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
                return false;

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
