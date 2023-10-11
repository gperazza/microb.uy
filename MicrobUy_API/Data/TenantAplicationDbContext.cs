using MicrobUy_API.Models;
using MicrobUy_API.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace MicrobUy_API.Data
{
    public class TenantAplicationDbContext : DbContext
    {
        private readonly ITenantInstance _service;
        public int _tenant { get; set; }

        public TenantAplicationDbContext(DbContextOptions<TenantAplicationDbContext> options, ITenantInstance service) : base(options)
        {
            _service = service;
            _tenant = _service.TenantInstanceId;
        }

        public DbSet<TenantInstanceModel> TenantInstances { get; set; }
        public DbSet<UserModel> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>().HasQueryFilter(mt => mt.TenantInstanceId == _tenant);
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<UserModel>().ToList()) // Write tenant Id to table
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                    case EntityState.Modified:
                        entry.Entity.TenantInstanceId = _tenant;
                        break;
                }
            }

            var result = base.SaveChanges();
            return result;
        }
    }
}