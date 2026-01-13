using System.ComponentModel.DataAnnotations;

namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class RegistrationModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
        public string? Initials { get; set; }
        public string? Role { get; set; } // Optional - assign a role if needed

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string OTP { get; set; } = string.Empty;

    }
}
