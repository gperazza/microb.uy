using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicrobUy_API.Models
{
    [PrimaryKey(nameof(UserId), nameof(UserName))]
    public class UserModel 
    {
        [ForeignKey(nameof(TenantInstanceId))]
        public int TenantInstanceId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; } 
        public string Biography { get; set; }
        public string Occupation { get; set; } 
        public CityModel City { get; set; }
        public DateTime Birthday { get; set; }
        public bool IsSanctioned { get; set; }
        public DateTime CreationDate { get; set; }
        public ICollection<TenantInstanceModel> AdministratedInstances { get; set; } = new List<TenantInstanceModel>();
        [NotMapped]
        public virtual ICollection<UserModel> Following { get; set; } = new List<UserModel>();
        [NotMapped]
        public virtual ICollection<UserModel> Followers { get; set; } = new List<UserModel>();
        [NotMapped]
        public ICollection<UserModel> BlockUsers { get; set; } = new List<UserModel>();
        [NotMapped]
        public ICollection<UserModel> MuteUsers { get; set; } = new List<UserModel>();
        //One to many del usuario que postea
        [NotMapped]
        public ICollection<PostModel> Posts { get; } = new List<PostModel>();
        //Post a los que le dio like
        [NotMapped]
        public ICollection<PostModel> Likes { get; set; } = new List<PostModel>();
    }
}