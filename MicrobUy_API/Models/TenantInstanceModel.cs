using MicrobUy_API.Dtos.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicrobUy_API.Models
{
    public class TenantInstanceModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TenantInstanceId { get; set; }
        public string Nombre { get; set; }
        public string Dominio { get; set; }
        public string Logo { get; set; }
        public bool Activo { get; set; }
        public string Tematica { get; set; }
        public string Description { get; set; }
        public EsquemaColoresEnum EsquemaColores { get; set; }
        public PrivacidadEnum Privacidad { get; set; }
        public ICollection<UserModel> InstanceAdministrators { get; set; }
    }
}