using PhytoIntellect.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Core.Entities;

public class Patient
{
    public int PatientId { get; set; }
    public int UserId { get; set; }
    public DateOnly? BirthDate { get; set; } 
    public Gender Gender { get; set; }

    // Navigation Properties
    public User? User { get; set; }
    public MedicalHistory? MedicalHistory { get; set; }
}
