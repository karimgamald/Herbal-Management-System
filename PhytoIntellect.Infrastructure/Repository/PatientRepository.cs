using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;
using PhytoIntellect.Infrastructure.Presistence;
using Microsoft.EntityFrameworkCore; // 👈 ده اللي هيطير الإيرور الأحمر
namespace PhytoIntellect.Infrastructure.Repository;

public class PatientRepository(ApplicationDbContext context) : Repository<Patient>(context), IPatientRepository
{
    public async Task<int> GetIdByUserIdAsync(string userId)
    {
        // 1. بنحول الـ string اللي جاي من التوكن لـ رقم (int)
        if (!int.TryParse(userId, out int parsedUserId))
            return 0; // لو فشل التحويل (مش رقم)، نرجع 0

        // 2. نقارن الرقم بالرقم عادي جداً
        var patient = await context.Patients
                                   .FirstOrDefaultAsync(p => p.UserId == parsedUserId);

        return patient?.PatientId ?? 0;
    }
}