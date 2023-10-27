using MicrobUy_API.Models;
using MicrobUy_API.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MicrobUy_API.Data
{
    public class IdentityProviderDbContext : IdentityDbContext<IdentityUser>
    {

        public IdentityProviderDbContext(DbContextOptions<IdentityProviderDbContext> options)
        : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
        }
    }
}