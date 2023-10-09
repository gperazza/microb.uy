using MicrobUy_API.Tenancy;

namespace MicrobUy_API.Middleware
{
    public class TenantResolver
    {
        private readonly RequestDelegate _next;
        public TenantResolver(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantInstance currentTenantService)
        {
            context.Request.Headers.TryGetValue("tenant", out var tenantFromHeader); // Obtiene el TestantInstanceId por el header 
            if (!string.IsNullOrEmpty(tenantFromHeader))
            {
                await currentTenantService.SetTenant(Convert.ToInt32(tenantFromHeader));
            }

            await _next(context);
        }
    }
}
