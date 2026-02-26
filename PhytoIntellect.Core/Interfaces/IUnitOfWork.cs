using PhytoIntellect.Core.Entities;
using System;

public interface IUnitOfWork : IDisposable
{
    ICRUDRepository<User> UserRepository { get; }
    ICRUDRepository<Patient> PatientRepository { get; }
    ICRUDRepository<Herbalist> HerbalistRepository { get; }

    ICRUDRepository<RefreshToken> RefreshTokenRepository { get; }
    Task<int> SaveChangesAsync();
}