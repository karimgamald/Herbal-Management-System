using PhytoIntellect.Core.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> UserRepository { get; }
    IRepository<Patient> PatientRepository { get; }
    IRepository<Herbalist> HerbalistRepository { get; }
    IRepository<RefreshToken> RefreshTokenRepository { get; }
    IRepository<MedicalHistory> MedicalHistoryRepository { get; }

    // ضفنا الـ CancellationToken هنا
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}