using PhytoIntellect.Application.Interfaces;
using PhytoIntellect.Core.Entities;
using PhytoIntellect.Infrastructure.Presistence;

namespace PhytoIntellect.Infrastructure.Repository;

public class FeedbackRepository : Repository<Feedback>, IFeedbackRepository
{
    private readonly ApplicationDbContext _context;

    public FeedbackRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}