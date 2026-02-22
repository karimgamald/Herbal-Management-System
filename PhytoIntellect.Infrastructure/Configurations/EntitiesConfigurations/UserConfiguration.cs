using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Configurations.EntitiesConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.UserId);

        builder.Property(u => u.UserName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.Password)
               .IsRequired();

        builder.Property(u => u.Role)
               .IsRequired()
               .HasMaxLength(50);
    }
}
