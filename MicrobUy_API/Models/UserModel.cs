using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MicrobUy_API.Models
{
    public class UserModel : IdentityUser
    {
        public int TenantInstanceId { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; } 
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
