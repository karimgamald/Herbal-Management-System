using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Configurations;

public class HerbConfiguration : IEntityTypeConfiguration<Herb>
{
    public void Configure(EntityTypeBuilder<Herb> builder)
    {
        // تحديد أطوال النصوص عشان منستهلكش مساحة عالفاضي
        builder.Property(h => h.HerbName).HasMaxLength(100).IsRequired();
        builder.Property(h => h.ScientificName).HasMaxLength(150);
        builder.Property(h => h.Dosage).HasMaxLength(100);

        // العشبة الجديدة بتكون قيد المراجعة افتراضياً
        builder.Property(h => h.IsApproved).HasDefaultValue(false);

        // علاقة العطار اللي ضاف العشبة (ممكن تكون Null لو السيستم هو اللي ضايفها)
        builder.HasOne(h => h.AddedByHerbalist)
               .WithMany() // مفيش داعي نعمل Collection في كلاس العطار للأعشاب اللي اقترحها
               .HasForeignKey(h => h.AddedByHerbalistId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasData(GetPredefinedHerbs());
    }

    private static List<Herb> GetPredefinedHerbs()
    {
        return new List<Herb>
        {
            new Herb { HerbId = 1, HerbName = "Ginger", ScientificName = "Zingiber officinale", Description = "A widely used root known for its spicy flavor and medicinal properties.", Benefits = "Relieves nausea, reduces inflammation, and aids digestion.", Dosage = "1-3 grams daily", Warnings = "High doses may cause heartburn or interact with blood thinners.", IsApproved = true },
            new Herb { HerbId = 2, HerbName = "Chamomile", ScientificName = "Matricaria chamomilla", Description = "A daisy-like plant commonly used to make herb infusions.", Benefits = "Promotes sleep, reduces anxiety, and soothes stomach aches.", Dosage = "1-2 cups of tea daily", Warnings = "May cause allergic reactions in people sensitive to ragweed.", IsApproved = true },
            new Herb { HerbId = 3, HerbName = "Turmeric", ScientificName = "Curcuma longa", Description = "A bright yellow spice widely used in Indian cuisine and Ayurvedic medicine.", Benefits = "Powerful anti-inflammatory and antioxidant effects.", Dosage = "500-2000 mg daily (with black pepper)", Warnings = "Can cause stomach upset in large amounts.", IsApproved = true },
            new Herb { HerbId = 4, HerbName = "Peppermint", ScientificName = "Mentha piperita", Description = "A hybrid mint cross between watermint and spearmint.", Benefits = "Relieves irritable bowel syndrome (IBS), eases headaches, and clears congestion.", Dosage = "1-2 cups of tea or 0.2ml essential oil capsule", Warnings = "May worsen acid reflux (GERD).", IsApproved = true },
            new Herb { HerbId = 5, HerbName = "Garlic", ScientificName = "Allium sativum", Description = "A pungent bulb used extensively in cooking and traditional medicine.", Benefits = "Boosts immune system, reduces blood pressure, and improves cholesterol levels.", Dosage = "1-2 cloves raw daily", Warnings = "Bad breath, heartburn, and may increase bleeding risk.", IsApproved = true },
            new Herb { HerbId = 6, HerbName = "Ashwagandha", ScientificName = "Withania somnifera", Description = "An ancient medicinal herb classified as an adaptogen.", Benefits = "Reduces stress and cortisol levels, boosts brain function.", Dosage = "300-500 mg root extract daily", Warnings = "Not recommended for pregnant women or those with autoimmune diseases.", IsApproved = true },
            new Herb { HerbId = 7, HerbName = "Echinacea", ScientificName = "Echinacea purpurea", Description = "A flowering plant in the daisy family, popular for fighting flu.", Benefits = "Prevents and treats the common cold, boosts immunity.", Dosage = "300-500 mg daily during illness", Warnings = "May cause mild stomach upset or allergic reactions.", IsApproved = true },
            new Herb { HerbId = 8, HerbName = "Lavender", ScientificName = "Lavandula angustifolia", Description = "A fragrant purple flower known for its calming scent.", Benefits = "Reduces anxiety, promotes restful sleep, and heals minor burns (topical).", Dosage = "1 cup of tea or aromatherapy", Warnings = "Not recommended for young boys (hormonal effects) if used topically in large amounts.", IsApproved = true },
            new Herb { HerbId = 9, HerbName = "Ginseng", ScientificName = "Panax ginseng", Description = "A slow-growing plant with fleshy roots, popular in Chinese medicine.", Benefits = "Increases energy, lowers blood sugar, and improves cognitive function.", Dosage = "200-400 mg daily", Warnings = "Can cause insomnia or interact with diabetes medications.", IsApproved = true },
            new Herb { HerbId = 10, HerbName = "Rosemary", ScientificName = "Salvia rosmarinus", Description = "A fragrant evergreen herb native to the Mediterranean.", Benefits = "Improves memory and focus, promotes hair growth (topical).", Dosage = "1-2 cups of tea or used as a spice", Warnings = "Extremely high doses can trigger seizures.", IsApproved = true }
        };
    }
}