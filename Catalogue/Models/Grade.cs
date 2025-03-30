using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalogue.Models
{
    public class Grade
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("student_id")]
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public User Student { get; set; }

        [Column("course_id")]
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [Column("grade")]
        public decimal GradeValue { get; set; }

        [Column("graded_at")]
        public DateTime GradedAt { get; set; }
    }
}
