using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Auth
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}
