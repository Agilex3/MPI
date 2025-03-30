using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalogue.Models
{
    public class Course
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("teacher_id")]
        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }

        public User Teacher { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
