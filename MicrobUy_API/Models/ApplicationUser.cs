using Microsoft.AspNetCore.Identity;

namespace MicrobUy_API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int TenantInstanceId { get; set; }
    }
}
