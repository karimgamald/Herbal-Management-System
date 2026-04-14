using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Core.Interfaces;

public interface IPatientRepository : IRepository<Patient>
{
    Task<int> GetIdByUserIdAsync(string userId);
    Task<Patient> GetPatientWithHistoryAsync(int patientId);
}