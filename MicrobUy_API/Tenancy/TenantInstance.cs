using MicrobUy_API.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MicrobUy_API.Tenancy
{
    public class TenantInstance : ITenantInstance
    {
        public int TenantInstanceId { get; set; }
    }
}

