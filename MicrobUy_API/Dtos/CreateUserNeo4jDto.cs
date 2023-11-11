using MicrobUy_API.Models;

namespace MicrobUy_API.Dtos
{
    public class CreateUserNeo4jDto
    {
        public int UserId { get; set; }
        public int TenantId { get; set; }
        public string UserName { get; set; }
        public string Occupation { get; set; }
        public string City { get; set; }
        public DateTime Birthday { get; set; }
        public bool IsSanctioned { get; set; }
        public DateTime CreationDate { get; set; }
    }
}

