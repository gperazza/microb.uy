using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicrobUy_API.Models
{
    public class UserModel
    {
        public int TenantInstanceId { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; }
        //One to many del usuario que postea
        public ICollection<PostModel> Posts { get; } = new List<PostModel>();

        //Post a los que le dio like
        public ICollection<PostModel> Likes { get; set; } = new List<PostModel>();
        public ICollection<TenantInstanceModel> AdministratedInstances { get; set; }
    }
}