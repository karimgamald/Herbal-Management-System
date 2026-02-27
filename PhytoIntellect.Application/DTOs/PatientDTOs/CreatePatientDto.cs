using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.DTOs.PatientDTOs;

public class CreatePatientDto
{
    public int UserId { get; set; }
    public int MedicalHistoryId { get; set; }
    public DateTime BirthDate { get; set; }
    public int Gender { get; set; } // رقم الـ Enum
}
