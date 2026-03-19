using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task BanUserAsync(int id);

        Task DeleteUserAsync(int id);
        Task UpdateUserAsync(UserDto userDto);

        Task<IEnumerable<ProductDto>> GetProductsAsync();
    }
}
