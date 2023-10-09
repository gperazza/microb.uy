using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MicrobUy_API.Tenancy
{
    public interface ITenantInstance
    {
        Task<bool> SetTenant(int? tenant);

        public int? TenantInstanceId { get; set; }
    }
}