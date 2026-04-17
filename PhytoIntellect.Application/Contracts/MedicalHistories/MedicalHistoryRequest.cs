using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.MedicalHistories;

public class MedicalHistoryRequest
{
    public bool Diabetes { get; set; } = false;
    public bool Hypertension { get; set; } = false;
    public bool Asthma { get; set; } = false;
    public bool HeartDisease { get; set; } = false;
    public bool KidneyDisease { get; set; } = false;
    public bool LiverDisease { get; set; } = false;
    public bool Smoker { get; set; } = false;
    public bool Pregnancy { get; set; } = false;
    public string? OtherNotes { get; set; } = string.Empty;
}
