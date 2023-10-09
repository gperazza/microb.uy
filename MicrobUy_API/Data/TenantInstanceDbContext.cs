using MicrobUy_API.Models;
using Microsoft.EntityFrameworkCore;

namespace MicrobUy_API.Data
{
    public class TenantInstanceDbContext : DbContext
    {
        // Este context es para validar que el tenant exista.
        public TenantInstanceDbContext(DbContextOptions<TenantInstanceDbContext> options)
        : base(options)
        {
        }

        public DbSet<TenantInstanceModel> TenantInstances { get; set; }
    }
}
