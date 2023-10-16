using MicrobUy_API.Models;
using MicrobUy_API.Tenancy;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MicrobUy_API.Data
{
    public class IdentityProviderDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly ITenantInstance _service;
        public int _tenant { get; set; }

        public IdentityProviderDbContext(DbContextOptions<IdentityProviderDbContext> options, ITenantInstance service)
        : base(options)
        {
            _service = service;
            _tenant = _service.TenantInstanceId;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>().HasQueryFilter(mt => mt.TenantInstanceId == _tenant);
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
        }
    }
}