using MicrobUy_API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MicrobUy_API.Dtos.PostDto
{
    public class PostDto
    {
        public int PostId { get; set; }
        public string Text { get; set; }
        public string Attachment { get; set; }
        public UserPostDto UserOwner { get; set; } = null!;
        public DateTime Created { get; set; };
        public bool isSanctioned { get; set; }
        public DateTime Created { get; set; }
        //Respuestas a un post
        public ICollection<PostDto> Comments { get; set; } = new List<PostDto>();
        //Personas que le dieron me gusta al post
        public ICollection<UserPostDto> Likes { get; set; } = new List<UserPostDto>();
        public List<string> Hashtag { get; set; } = new List<string>();
    }
}
