using Blog.Application.DTOs;
using Blog.Application.Interfaces.Auth;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;
using Blog.Domain.Entities;

namespace Blog.Application.Services
{
    public class AuthService : IAuthService 
    {
        private readonly IAuthRepository _userRepo;
        private readonly IJwtTokenService _jwt;

        public AuthService(IAuthRepository userRepo, IJwtTokenService jwt)
        {
            _userRepo = userRepo;
            _jwt = jwt;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var existUser = await _userRepo.GetByEmailAsync(dto.EmailId!);
            if (existUser == null)
            {
                var user = new User
                {
                    FName = dto.FName,
                    LName = dto.LName,
                    EmailId = dto.EmailId,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    RefreshToken = _jwt.GenerateRefreshToken(),
                    Expires = DateTime.UtcNow.AddDays(7)
                };

                await _userRepo.AddAsync(user);
                await _userRepo.SaveChangesAsync();
                return new AuthResponseDto(
                        _jwt.GenerateAccessToken(user),
                        user.RefreshToken,
                        user.Role
                    );
            }
            else
            {
                return new AuthResponseDto(
                        "",
                        "",
                        ""
                    );
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.EmailId!);
            if (user == null)
            {
                return new AuthResponseDto(
                        "",
                        "",
                        ""
                    );
            }
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new Exception("Invalid credentials");

            user.RefreshToken = _jwt.GenerateRefreshToken();
            user.Expires = DateTime.UtcNow.AddDays(7);
            await _userRepo.SaveChangesAsync();
            return new AuthResponseDto(
                    _jwt.GenerateAccessToken(user),
                    user.RefreshToken,
                    user.Role
                );
        }
        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userRepo.GetByRefreshTokenAsync(refreshToken);

            if (user == null)
                throw new Exception("Invalid refresh token");

            user.RefreshToken = _jwt.GenerateRefreshToken();
            user.Expires = DateTime.UtcNow.AddDays(7);
            await _userRepo.SaveChangesAsync();

            return new AuthResponseDto
            (
                    _jwt.GenerateAccessToken(user),
                    user.RefreshToken,
                    user.Role
                );
        }
    }
}
