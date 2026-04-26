using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Configurations;

public class UserFavoriteConfiguration : IEntityTypeConfiguration<UserFavorite>
{
    public void Configure(EntityTypeBuilder<UserFavorite> builder)
    {
        builder.ToTable("UserFavorites");

        builder.HasKey(f => f.Id);

        // 3. الربط مع جدول المستخدمين (User)
        builder.HasOne(f => f.User)
               .WithMany()
               .HasForeignKey(f => f.UserId)
               .OnDelete(DeleteBehavior.Cascade); 

        // 4. عمل Index (فهرس) ثلاثي 🚀
        // ده أهم سطر في الـ Config لأنه بيخلي الـ Query طلقة 
        // وبيمنع إن اليوزر يضيف نفس الحاجة مرتين في المفضلة (Unique Index)
        builder.HasIndex(f => new { f.UserId, f.TargetId, f.Type })
               .IsUnique();

        // 5. ضبط التاريخ
        builder.Property(f => f.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");
    }
}