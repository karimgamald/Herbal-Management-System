using PhytoIntellect.Core.Entities;
using PhytoIntellect.Infrastructure.Presistence;

namespace PhytoIntellect.Infrastructure.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public ICRUDRepository<User> UserRepository { get; private set; }
        public ICRUDRepository<Patient> PatientRepository { get; private set; }
        public ICRUDRepository<Herbalist> HerbalistRepository { get; private set; }
        public ICRUDRepository<RefreshToken> RefreshTokenRepository { get; private set; }

        //Injection
        public UnitOfWork(ApplicationDbContext context,ICRUDRepository<User> userRepository
            ,ICRUDRepository<Patient> patientRepository,ICRUDRepository<Herbalist> herbalistRepository,
            ICRUDRepository<RefreshToken> refreshTokenRepository)
        {
            _context = context;
            UserRepository = userRepository;
            PatientRepository = patientRepository;
            HerbalistRepository = herbalistRepository;
            RefreshTokenRepository = refreshTokenRepository;
        }

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();
        public void Dispose()
            => _context.Dispose();
    }
}
