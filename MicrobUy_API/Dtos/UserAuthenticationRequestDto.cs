using System.ComponentModel.DataAnnotations;

namespace MicrobUy_API.Dtos
{
    public class UserAuthenticationRequestDto
    {
        [Required(ErrorMessage = "Email es requerido.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password es requerido.")]
        public string? Password { get; set; }
    }
}