using MicrobUy_API.Models;

namespace MicrobUy_API.Dtos
{
    public class CreatePostDto
    {
        public int PostId { get; set; }
        public string Text { get; set; }
        public string Attachment { get; set; }
        public bool isSanctioned { get; set; }
        public List<string> Hashtag { get; set; } = new List<string>();
    }
}
