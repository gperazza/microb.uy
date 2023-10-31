using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace MicrobUy_API.Models
{
    public class PostModel
    {
        [ForeignKey(nameof(TenantInstanceId))]
        public int TenantInstanceId { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostId { get; set; }
        public string Text { get; set; }
        public string Attachment { get; set; }
        public UserModel UserOwner { get; set; } = null!;
        public bool isSanctioned { get; set; }
        public DateTime Created { get; set; }
        public bool isComment { get; set; } 
        //Respuestas a un post
        public ICollection<PostModel> Comments { get; set; } = new List<PostModel>();
        //Personas que le dieron me gusta al post
        public ICollection<UserModel> Likes { get; set; } = new List<UserModel>();
        public List<string> Hashtag { get; set; } = new List<string>();
    }
}
