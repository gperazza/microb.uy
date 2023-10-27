using MicrobUy_API.Dtos.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicrobUy_API.Models
{
    [PrimaryKey(nameof(TenantInstanceId), nameof(Dominio))]
    public class TenantInstanceModel
    {
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
        public ICollection<UserModel> InstanceAdministrators { get; set; } = new List<UserModel>();
        public DateTime CreationDate { get; set; }
    }
}