using TestService.DTOs;
using TestService.Models;

namespace TestService.Services
{
    public interface IAuthService
    {
        Task<(User? User, string? Error)> RegisterAsync(RegisterDto dto);
        Task<(User? User, string? Error)> LoginAsync(LoginDto dto);
        string GenerateToken(User user);
    }
}