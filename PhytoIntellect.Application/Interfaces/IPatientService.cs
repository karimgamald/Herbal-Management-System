using PhytoIntellect.Application.Contracts.Patients;

namespace PhytoIntellect.Application.Interfaces;

public interface IPatientService
{
    Task<PatientRequest?> GetMyProfileAsync(int userId, CancellationToken cancellationToken = default);
    Task<string> UpdateMyProfileAsync(int userId, UpdatePatientRequest request, CancellationToken cancellationToken = default);

    Task<PatientRequest?> GetPatientByIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PatientRequest>> GetAllPatientsAsync(CancellationToken cancellationToken = default);
}