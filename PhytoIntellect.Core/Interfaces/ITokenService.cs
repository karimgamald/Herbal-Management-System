using Herbal_System.Entities;

namespace Herbal_System.Interfaces
{
    public interface ITokenService
    {
        string CreateAccessToken(User user);
        string CreateRefreshToken();
    }

}
