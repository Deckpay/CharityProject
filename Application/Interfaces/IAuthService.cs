using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDto registerDto);
        Task<User?> LoginAsync(string emailOrUserName, string password);
        Task<IEnumerable<County>> GetCountiesAsync();
        Task<IEnumerable<ProductCategory>> GetProductCategoriesAsync();
    }
}
