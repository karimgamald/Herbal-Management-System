using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;


namespace PhytoIntellect.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.UserName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
               .IsRequired();

        builder.Property(u => u.Role)
               .IsRequired()
               .HasMaxLength(50);

        DataSeedAdmin(builder);
    }

    private void DataSeedAdmin(EntityTypeBuilder<User> builder)
    {
        var superAdmin = new User
        {
            Id = 999,
            FullName = "Super Admin",
            UserName = "super_admin",
            Email = "herbal.ai200@gmail.com",
            PasswordHash = "$2a$12$uiBZ/NOR7RYt6xd.NBX.J./x.IlrlVx3IDp8GZQ3pstkzOckPTtLK",
            Phone = "01000000000",
            Role = "Admin",
            Governorate = "Menofia",
            City = "Menofia",
            Street = "Menofia",
            IsEmailConfirmed = true,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)

        };

        builder.HasData(superAdmin);
    }
}
