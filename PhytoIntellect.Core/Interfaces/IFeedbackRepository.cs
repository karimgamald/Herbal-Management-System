using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Interfaces;

public interface IFeedbackRepository : IRepository<Feedback>
{
    // مش محتاجين نكتب دوال زيادة هنا، الـ IRepository فيه الـ Get والـ Create وكل حاجة
}