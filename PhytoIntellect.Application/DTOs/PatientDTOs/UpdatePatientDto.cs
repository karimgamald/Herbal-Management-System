using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.DTOs.PatientDTOs;

// للتعديل واستكمال البيانات
public class UpdatePatientDto
{
    public DateOnly BirthDate { get; set; }
    // هنفترض إن الـ Gender عبارة عن Enum، فبناخده كـ int من الموبايل
    public int Gender { get; set; }
}