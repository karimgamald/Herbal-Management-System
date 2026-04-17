using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Patients;

public class PatientRequest
{
    public int PatientId { get; set; }
    public int UserId { get; set; }
    public int? MedicalHistoryId { get; set; } 
    public string BirthDate { get; set; }
    public string GenderName { get; set; } = string.Empty;
    public int Age { get; set; }
}