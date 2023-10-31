using MicrobUy_API.Models;
using MicrobUy_API.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Hosting;

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
        public DbSet<PostModel> Post { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>().HasMany(e => e.Likes).WithMany(e => e.Likes).UsingEntity("PostLikes");
            modelBuilder.Entity<UserModel>().HasMany(e => e.AdministratedInstances).WithMany(e => e.InstanceAdministrators).UsingEntity("InstanceAdministrators");
            modelBuilder.Entity<UserModel>().HasMany(e => e.Posts).WithOne(e => e.UserOwner);
            modelBuilder.Entity<PostModel>().OwnsOne(x => x.Hashtag);
            modelBuilder.Entity<PostModel>().HasDiscriminator<string>("Discriminator").HasValue<PostModel>("PostModel").HasValue<CommentModel>("CommentModel");
            modelBuilder.Entity<UserModel>().HasQueryFilter(mt => mt.TenantInstanceId == _tenant);
            modelBuilder.Entity<PostModel>().HasQueryFilter(mt => mt.TenantInstanceId == _tenant);
        }

        public override int SaveChanges()
        {
          
            foreach (var entry in ChangeTracker.Entries().ToList()) // Write tenant Id to table
            {
                
                switch (entry.State)
                {
                    case EntityState.Added:
                    case EntityState.Modified:
                        if (entry.Entity.GetType() == typeof(UserModel)) {
                            UserModel newEntery = (UserModel)entry.Entity;
                            newEntery.TenantInstanceId = _tenant;
                        }
                        if (entry.Entity.GetType() == typeof(PostModel))
                        {
                            PostModel newEntery = (PostModel)entry.Entity;
                            newEntery.TenantInstanceId = _tenant;
                        }
                        break;
                }
            }

            var result = base.SaveChanges();
            return result;
        }

    }
}