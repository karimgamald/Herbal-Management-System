using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Configurations;

public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.HasKey(f => f.FeedbackId);

        // 1. تقييد التعليق
        builder.Property(f => f.Comment).HasMaxLength(1000);

        // 2. 🚨 منع التكرار (Unique Index): مفيش مريض يقيم نفس الوصفة مرتين!
        builder.HasIndex(f => new { f.RecipeId, f.PatientId }).IsUnique();

        // 3. 🚨 تقييد الأرقام (Check Constraint): التقييم لازم يكون بين 1 و 5 بس
        builder.ToTable("Feedbacks", t => t.HasCheckConstraint("CK_Feedback_RatingValue", "[RatingValue] >= 1 AND [RatingValue] <= 5"));

        // 4. تظبيط العلاقات
        builder.HasOne(f => f.Recipe)
               .WithMany(r => r.Feedbacks)
               .HasForeignKey(f => f.RecipeId)
               .OnDelete(DeleteBehavior.Cascade); // لو الوصفة اتمسحت، تقييماتها تتمسح

        builder.HasOne(f => f.Patient)
               .WithMany(p => p.Feedbacks)
               .HasForeignKey(f => f.PatientId)
               // استخدمنا Restrict عشان لو مسحنا المريض الداتابيز متضربش Multiple Cascade Paths
               .OnDelete(DeleteBehavior.Restrict);
    }
}