using MicrobUy_API.Models;
using MicrobUy_API.Tenancy;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MicrobUy_API.Data
{
    public class TenantAplicationDbContext : IdentityDbContext<UserModel>
    {
        public int? _tenant { get; }

        public TenantAplicationDbContext(DbContextOptions<TenantAplicationDbContext> options, ITenantInstance service)
            : base(options) => _tenant = service.TenantInstanceId;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserModel>().HasQueryFilter(mt => mt.TenantInstanceId == _tenant);
        }
       

        public DbSet<TenantInstanceModel> TenantInstances { get; set; }
        public DbSet<UserModel> User { get; set; }

        //public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        //{
        //    foreach (var entry in ChangeTracker.Entries<TenantInstanceModel>().ToList()) // Write tenant Id to table
        //    {
        //        switch (entry.State)
        //        {
        //            case EntityState.Added:
        //            case EntityState.Modified:
        //                entry.Entity.TenantInstanceId = _tenant;
        //                break;
        //        }
        //    }

        //    var result = await base.SaveChangesAsync(cancellationToken);
        //    return result;
        //}
    }
}