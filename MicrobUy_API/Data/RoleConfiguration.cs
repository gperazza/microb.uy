using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicrobUy_API.Data
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {   
                    Id = "1",
                    Name = "Common-User",
                    NormalizedName = "COMMON-USER"
                },
                new IdentityRole
                {
                    Id = "2",
                    Name = "Platform-Administrator",
                    NormalizedName = "PLATFORM-ADMINISTRATOR"
                },
                new IdentityRole
                {
                    Id = "3",
                    Name = "Instance-Administrator",
                    NormalizedName = "INSTANCE-ADMINISTRATOR"
                },
                new IdentityRole
                {
                    Id = "4",
                    Name = "Moderator",
                    NormalizedName = "MODERATOR"
                }
            );
        }
    }
}
