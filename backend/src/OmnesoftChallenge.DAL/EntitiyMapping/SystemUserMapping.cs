using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmnesoftChallenge.DAL.Entities;

namespace OmnesoftChallenge.DAL.EntitiyMapping;

public class SystemUserMapping : IEntityTypeConfiguration<SystemUser>
{
    public void Configure(EntityTypeBuilder<SystemUser> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.RefreshToken)
            .WithOne(x => x.SystemUser)
            .HasForeignKey(x => x.SystemUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.UserName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Password).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Role).IsRequired();

        builder.ToTable("system_user");
    }
}
