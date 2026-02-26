using PhytoIntellect.Core.Entities;

namespace PhytoIntellect.Application.Interfaces
{
    public interface ITokenService
    {
        string CreateAccessToken(User user);
        string CreateRefreshToken();
    }
}
