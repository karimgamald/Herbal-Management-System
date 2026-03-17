using PhytoIntellect.Core.Enums;

namespace PhytoIntellect.Application.DTOs.PatientDTOs;

public class UpdatePatientDto
{
    public string BirthDate { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
}