using PhytoIntellect.Core.Entities;
using PhytoIntellect.Infrastructure.Presistence;

namespace PhytoIntellect.Infrastructure.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IRepository<User> UserRepository { get; private set; }
        public IRepository<Patient> PatientRepository { get; private set; }
        public IRepository<Herbalist> HerbalistRepository { get; private set; }
        public IRepository<RefreshToken> RefreshTokenRepository { get; private set; }

        //Injection
        public UnitOfWork(ApplicationDbContext context,IRepository<User> userRepository
            ,IRepository<Patient> patientRepository,IRepository<Herbalist> herbalistRepository,
            IRepository<RefreshToken> refreshTokenRepository)
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
