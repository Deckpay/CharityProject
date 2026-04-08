using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDto registerDto);
        Task<User?> LoginAsync(string emailOrUserName, string password);
        Task<bool> DeleteMyAccountAsync(int userId);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
    }
}
