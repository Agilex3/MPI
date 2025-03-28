using System.ComponentModel.DataAnnotations;

namespace Catalogue.Models
{
    public class User
    {
        [Key]
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public DateTime created_at { get; set; }
    }
}
