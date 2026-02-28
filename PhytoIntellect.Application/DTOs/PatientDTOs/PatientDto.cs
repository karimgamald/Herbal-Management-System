using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.DTOs.PatientDTOs;

// للعرض
public class PatientDto
{
    public int PatientId { get; set; }
    public int UserId { get; set; }
    public int? MedicalHistoryId { get; set; } // Nullable عشان السيناريو بتاعنا
    public DateOnly BirthDate { get; set; }
    public string GenderName { get; set; } = string.Empty;
}
