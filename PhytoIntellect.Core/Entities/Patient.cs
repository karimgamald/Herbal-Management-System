using PhytoIntellect.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class Patient
{
    public int PatientId { get; set; }

    public int MedicalHistoryId { get; set; }
    public int UserId { get; set; }

    public DateTime BirthDate { get; set; } 
    public Gender Gender { get; set; } // يفضل قدام تحولها لـ Enum

    // Navigation Properties
    public User User { get; set; }
    public MedicalHistory MedicalHistory { get; set; }
}
