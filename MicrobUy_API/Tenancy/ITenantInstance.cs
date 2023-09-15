using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MicrobUy_API.Tenancy
{
    public interface ITenantInstance
    {
        public int TenantInstanceId { get; set; }
    }
}