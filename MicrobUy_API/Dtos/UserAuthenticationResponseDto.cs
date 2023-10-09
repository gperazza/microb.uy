namespace MicrobUy_API.Dtos
{
    public class UserAuthenticationResponseDto
    {
        public bool IsAuthSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Token { get; set; }
    }
}
