using System.ComponentModel.DataAnnotations;

namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;

    }
}
