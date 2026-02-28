using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.DTOs.MedicalHistoryDTOs;

// 2. DTO للإنشاء والتعديل (Upsert)
public class ManageMedicalHistoryDto
{
    public bool Diabetes { get; set; }
    public bool Hypertension { get; set; }
    public bool Asthma { get; set; }
    public bool HeartDisease { get; set; }
    public bool KidneyDisease { get; set; }
    public bool LiverDisease { get; set; }
    public bool Smoker { get; set; }
    public bool Pregnancy { get; set; }
    public string? OtherNotes { get; set; }
}