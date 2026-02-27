using PhytoIntellect.Application.DTOs;
using PhytoIntellect.Application.DTOs.PatientDTOs;

namespace PhytoIntellect.Application.Interfaces;

public interface IPatientService
{
    Task<string> CreatePatientAsync(CreatePatientDto request, CancellationToken cancellationToken = default);
    Task<PatientDto?> GetPatientByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PatientDto>> GetAllPatientsAsync(CancellationToken cancellationToken = default);
    Task<string> UpdatePatientAsync(int id, UpdatePatientDto request, CancellationToken cancellationToken = default);
    Task<string> DeletePatientAsync(int id, CancellationToken cancellationToken = default);
}