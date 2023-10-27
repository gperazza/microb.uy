using System.ComponentModel.DataAnnotations;

namespace MicrobUy_API.Dtos
{
    public class UserRegistrationRequestDto
    {
        [Required(ErrorMessage = "Email is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string LastName { get; set; }
        public string ProfileImage { get; set; }
        public string Biography { get; set; }
        public string Occupation { get; set; }
        public string City { get; set; }
        public DateTime Birthday { get; set; }
        public bool IsSanctioned { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }
    }
}