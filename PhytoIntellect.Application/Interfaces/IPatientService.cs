using PhytoIntellect.Application.DTOs;
using PhytoIntellect.Application.DTOs.PatientDTOs;

namespace PhytoIntellect.Application.Interfaces;

public interface IPatientService
{
    // للمريض
    Task<PatientDto?> GetMyProfileAsync(int userId, CancellationToken cancellationToken = default);
    Task<string> UpdateMyProfileAsync(int userId, UpdatePatientDto request, CancellationToken cancellationToken = default);

    // للإدارة والعشابين
    Task<PatientDto?> GetPatientByIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PatientDto>> GetAllPatientsAsync(CancellationToken cancellationToken = default);
}