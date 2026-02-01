using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Application.DTOs
{
    public record LoginDto(string EmailId, string Password);
    public record RegisterDto(string EmailId, string Password, string FName, string LName);
    public record AuthResponseDto(string AccessToken, string RefreshToken, string Role);
    public record RefreshTokenDto(string Token);
    public record GenerateRefreshToken(string RefreshToken, DateTime expiryDate);
}
