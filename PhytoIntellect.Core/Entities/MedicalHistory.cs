using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class MedicalHistory
{
    public int MedicalHistoryId { get; set; }
    public int PatientId { get; set; }

    public bool Diabetes { get; set; }
    public bool Hypertension { get; set; }
    public bool Asthma { get; set; }
    public bool HeartDisease { get; set; }
    public bool KidneyDisease { get; set; }
    public bool LiverDisease { get; set; }
    public bool Smoker { get; set; }
    public bool Pregnancy { get; set; }
    public string? OtherNotes { get; set; }

    public Patient Patient { get; set; }
}
