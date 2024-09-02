using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
