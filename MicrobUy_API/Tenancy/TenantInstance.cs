using MicrobUy_API.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MicrobUy_API.Tenancy
{
    public class TenantInstance : ITenantInstance
    {
        private readonly TenantInstanceDbContext _context;

        public TenantInstance(TenantInstanceDbContext context)
        {
            _context = context;

        }
        public async Task<bool> SetTenant(int? tenant)
        {

            var tenantInfo = await _context.TenantInstances.Where(x => x.TenantInstanceId == tenant).FirstOrDefaultAsync(); // se chequea si el TenantInstanceId existe
            if (tenantInfo != null)
            {
                TenantInstanceId = tenant;
                return true;
            }
            else
            {
                throw new Exception("Tenant invalid");
            }

        }
        public int? TenantInstanceId { get; set; }
    }
}

