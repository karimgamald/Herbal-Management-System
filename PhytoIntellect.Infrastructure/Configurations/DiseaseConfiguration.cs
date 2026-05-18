using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhytoIntellect.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Infrastructure.Configurations;

public class DiseaseConfiguration : IEntityTypeConfiguration<Disease>
{
    public void Configure(EntityTypeBuilder<Disease> builder)
    {
        builder.HasKey(d => d.DiseaseId);
        builder.Property(d => d.DiseaseName).HasMaxLength(150).IsRequired();
        builder.Property(d => d.DiseaseType).HasMaxLength(100);

        builder.HasData(
            new Disease { DiseaseId = 1, DiseaseName = "Insomnia", DiseaseType = "Neurological", Description = "Habitual sleeplessness or inability to sleep.", Symptoms = "Difficulty falling asleep, waking up often." },
            new Disease { DiseaseId = 2, DiseaseName = "Irritable Bowel Syndrome (IBS)", DiseaseType = "Gastrointestinal", Description = "A common disorder that affects the large intestine.", Symptoms = "Cramping, abdominal pain, bloating, gas." },
            new Disease { DiseaseId = 3, DiseaseName = "Common Cold", DiseaseType = "Respiratory", Description = "A viral infection of your nose and throat.", Symptoms = "Runny nose, sore throat, cough, congestion." },
            new Disease { DiseaseId = 4, DiseaseName = "Anxiety", DiseaseType = "Psychological", Description = "A feeling of worry, nervousness, or unease.", Symptoms = "Restlessness, rapid breathing, increased heart rate." },
            new Disease { DiseaseId = 5, DiseaseName = "Indigestion", DiseaseType = "Gastrointestinal", Description = "Discomfort in your upper abdomen.", Symptoms = "Bloating, nausea, belching." }
        );
    }
}