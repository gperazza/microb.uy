namespace MicrobUy_API.Dtos
{
    public class CrearPostNeo4jDto
    {
        public int userId { get; set; }
        public int tenantId { get; set; }
        public int postId { get; set; }
        public DateTime postCreated {  get; set; }
        public bool isSanctioned { get; set; }
        public List<string> hashtags { get; set; } = new List<string>();
    }
}
