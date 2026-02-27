using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.DTOs.PatientDTOs;

public class UpdatePatientDto
{
    public DateTime BirthDate { get; set; }
    public int Gender { get; set; }
}
