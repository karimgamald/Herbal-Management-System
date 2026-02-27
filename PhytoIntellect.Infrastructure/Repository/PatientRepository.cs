using PhytoIntellect.Core.Entities;
using PhytoIntellect.Core.Interfaces;
using PhytoIntellect.Infrastructure.Presistence;

namespace PhytoIntellect.Infrastructure.Repository;

public class PatientRepository(ApplicationDbContext context) : Repository<Patient>(context), IPatientRepository
{
}