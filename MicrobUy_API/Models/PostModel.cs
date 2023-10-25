using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace MicrobUy_API.Models
{
    public class PostModel
    {
        public int TenantInstanceId { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostId { get; set; }
        public String Text { get; set; }
        public String Attachment { get; set; }
        public List<String> Hashtag { get; set; } = new List<String>();
        //Personas que le dieron me gusta al post
        public ICollection<UserModel> Likes { get; set; } = new List<UserModel>();
        //One to many del usuario que postea
        public int UserId { get; set; }
        public UserModel User { get; set; } = null!;
        //Respuestas a un post
        public ICollection<PostModel> Comments { get; set; } = new List<PostModel>();
        public bool isSanctioned { get; set; }
        
    }
}
