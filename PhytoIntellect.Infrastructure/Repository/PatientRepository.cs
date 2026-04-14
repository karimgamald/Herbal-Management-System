using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;
using PhytoIntellect.Infrastructure.Presistence;
using Microsoft.EntityFrameworkCore;
namespace PhytoIntellect.Infrastructure.Repository;

public class PatientRepository(ApplicationDbContext context) : Repository<Patient>(context), IPatientRepository
{
    public async Task<int> GetIdByUserIdAsync(string userId)
    {
        if (!int.TryParse(userId, out int parsedUserId))
            return 0; 

        var patient = await context.Patients.FirstOrDefaultAsync(p => p.UserId == parsedUserId);
        return patient?.PatientId ?? 0;
    }
    public async Task<Patient> GetPatientWithHistoryAsync(int patientId)
    {
        return await context.Patients
            .Include(p => p.MedicalHistory) 
            .FirstOrDefaultAsync(p => p.PatientId == patientId);
    }
}