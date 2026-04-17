using System;
using System.Collections.Generic;
using System.Text;

namespace PhytoIntellect.Application.Contracts.Patients;

public class UpdatePatientRequest
{
    public string BirthDate { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
}
