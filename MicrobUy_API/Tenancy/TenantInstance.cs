using MicrobUy_API.Data;
using Microsoft.EntityFrameworkCore;

namespace MicrobUy_API.Tenancy
{
    public class TenantInstance : ITenantInstance
    {
        private readonly TenantInstanceDbContext _context;

        //Este tenant es para las entidades que se generen sin tenant ejemplo los usuario de la plataforma
        private static readonly int TENANT_BASE = 0;

        public TenantInstance(TenantInstanceDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SetTenant(int tenant)
        {
            var tenantInfo = await _context.TenantInstances.Where(x => x.TenantInstanceId == tenant).FirstOrDefaultAsync(); // se chequea si el TenantInstanceId existe
            if (tenantInfo != null || tenant == TENANT_BASE)
            {
                TenantInstanceId = tenant;
                return true;
            }
            else
            {
                throw new Exception("Tenant invalid");
            }
        }

        public int TenantInstanceId { get; set; }
    }
}