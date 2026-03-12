using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Configurations;

public class RecipeHerbConfiguration : IEntityTypeConfiguration<RecipeHerb>
{
    public void Configure(EntityTypeBuilder<RecipeHerb> builder)
    {
        builder.HasKey(rh => rh.RecipeHerbId);

        // منع الـ Cascade Delete عشان لو مسحنا عشبة من القاموس، الوصفات القديمة متتمسحش
        builder.HasOne(rh => rh.Recipe)
               .WithMany(r => r.RecipeHerbs)
               .HasForeignKey(rh => rh.RecipeId)
               .OnDelete(DeleteBehavior.Cascade); // مسح الوصفة يمسح مقاديرها

        builder.HasOne(rh => rh.Herb)
               .WithMany(h => h.RecipeHerbs)
               .HasForeignKey(rh => rh.HerbId)
               .OnDelete(DeleteBehavior.Restrict); // لكن مسح العشبة ميمسحش الوصفة
    }
}
