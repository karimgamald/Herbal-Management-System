using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.DTOs.PatientDTOs;

public class PatientDto
{
    public int PatientId { get; set; }
    public DateTime BirthDate { get; set; }
    public string GenderName { get; set; }
}
