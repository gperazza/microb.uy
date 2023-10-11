using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MicrobUy_API.Data
{
    public class IdentityProviderDbContext : IdentityDbContext
    {
        public IdentityProviderDbContext(DbContextOptions<IdentityProviderDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}