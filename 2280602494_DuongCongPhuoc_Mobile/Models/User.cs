using Microsoft.AspNetCore.Identity;

namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class User : IdentityUser
    {
        public string? Initials { get; set; }
        //User = IdentityUser + string Inititals
    }

}
