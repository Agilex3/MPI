using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Catalogue.Models
{
    public class GradeDTO
    {
        public int StudentId { get; set; }
        //public User Student { get; set; }

        public int CourseId { get; set; }
        //public Course Course { get; set; }

        public decimal GradeValue { get; set; }

        public DateTime GradedAt { get; set; }
    }
}
