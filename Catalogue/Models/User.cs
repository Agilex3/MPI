using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using BCrypt.Net;


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



        public void SetPassword(string newPassword)
        {
            password = newPassword; // NU se face hashing (NESIGUR!)
        }

        public bool VerifyPassword(string inputPassword)
        {
            return password == inputPassword; // Comparare directă (NESIGUR!)
        }
    }
}
