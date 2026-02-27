using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Core.Interfaces;

public interface IPatientRepository : IRepository<Patient>
{
    // لو فيه ميثودز مخصصة للـ Patient غير الـ CRUD هتنزل هنا قدام
}