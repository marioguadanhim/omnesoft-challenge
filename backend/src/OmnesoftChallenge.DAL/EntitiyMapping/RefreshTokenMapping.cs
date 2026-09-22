using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmnesoftChallenge.DAL.Entities;

namespace OmnesoftChallenge.DAL.EntitiyMapping;

public class RefreshTokenMapping : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.HasOne(x => x.SystemUser)
            .WithMany(x => x.RefreshToken)
            .HasForeignKey(x => x.SystemUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Token)
            .IsUnique();

        builder.ToTable("refresh_token");
    }
}
