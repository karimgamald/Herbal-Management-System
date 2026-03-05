using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PhytoIntellect.Application.DTOs.MedicalHistoryDTOs;

// 2. DTO للإنشاء والتعديل (Upsert)
public class ManageMedicalHistoryDto
{
    [DefaultValue(false)]
    public bool Diabetes { get; set; }
    [DefaultValue(false)]
    public bool Hypertension { get; set; }
    [DefaultValue(false)]
    public bool Asthma { get; set; }
    [DefaultValue(false)]
    public bool HeartDisease { get; set; }
    [DefaultValue(false)]
    public bool KidneyDisease { get; set; }
    [DefaultValue(false)]
    public bool LiverDisease { get; set; }
    [DefaultValue(false)]
    public bool Smoker { get; set; }
    [DefaultValue(false)]
    public bool Pregnancy { get; set; }
    public string? OtherNotes { get; set; }
}