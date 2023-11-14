using MicrobUy_API.Models;

namespace MicrobUy_API.Dtos
{
    public class ModifyUserRequestDto
    {
        public int UserId { get; set; }  
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; }
        public DateTime Birthday { get; set; }
        public string Biography { get; set; }
        public string Occupation { get; set; }
        public CityModel City { get; set; }
    }
}
