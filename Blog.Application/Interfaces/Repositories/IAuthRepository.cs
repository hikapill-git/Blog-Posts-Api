using Blog.Application.DTOs;
using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Repositories
{
    public interface IAuthRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByRefreshTokenAsync(string refreshToken);
        Task AddAsync(User user);
        Task SaveChangesAsync();

    }
}
