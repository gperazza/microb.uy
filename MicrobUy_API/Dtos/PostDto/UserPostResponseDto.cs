namespace MicrobUy_API.Dtos.PostDto
{
    public class UserPostResponseDto
    {
        public bool IsSuccessfulPosted { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
