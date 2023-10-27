namespace MicrobUy_API.Dtos
{
    public class UserPostResponseDto
    {
        public bool IsSuccessfulPosted { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
