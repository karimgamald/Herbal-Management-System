using PhytoIntellect.Core.Entities;
using System;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> UserRepository { get; }
    IRepository<Patient> PatientRepository { get; }
    IRepository<Herbalist> HerbalistRepository { get; }

    IRepository<RefreshToken> RefreshTokenRepository { get; }
    Task<int> SaveChangesAsync();
}