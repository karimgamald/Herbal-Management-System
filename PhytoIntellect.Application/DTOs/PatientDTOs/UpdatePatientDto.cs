using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.DTOs.PatientDTOs;

public class UpdatePatientDto
{
    public DateOnly BirthDate { get; set; }
    public PhytoIntellect.Core.Enums.Gender Gender { get; set; } 
}