using MicrobUy_API.Models;

namespace MicrobUy_API.Dtos
{
    public class CreatePostDto
    {
        public int UserId { get; set; }
        public String Text { get; set; }
        public String Attachment { get; set; }
        public List<String> Hashtag { get; set; } = new List<String>();   
    }
}
