using PhytoIntellect.Application.Contracts.Patients;
using PhytoIntellect.Application.Paginations;

namespace PhytoIntellect.Application.Interfaces;

public interface IPatientService
{
    Task<PatientRequest?> GetMyProfileAsync(int userId, CancellationToken cancellationToken = default);
    Task<string> UpdateMyProfileAsync(int userId, UpdatePatientRequest request, CancellationToken cancellationToken = default);

    Task<PatientRequest?> GetPatientByIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task<PaginatedList<PatientRequest>> GetAllPatientsAsync(RequestFilters filters,CancellationToken cancellationToken = default);
    Task<bool> DeletePatientAsync(int patientId, CancellationToken cancellationToken = default);
}