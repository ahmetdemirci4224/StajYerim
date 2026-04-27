using StajYerim.Core.Entities;

namespace StajYerim.Application.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}
