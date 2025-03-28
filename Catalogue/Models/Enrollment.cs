using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Catalogue.Models
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int StudentId { get; set; }
        public User Student { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        public DateTime EnrolledAt { get; set; }
    }
}
