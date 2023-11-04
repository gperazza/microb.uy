using MicrobUy_API.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicrobUy_API.Dtos.PostDto
{
    public class UserPostDto
    {
        public string UserName { get; set; }
        public string ProfileImage { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
