using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAdminService
    {
        // User
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task BanUserAsync(int id);
        Task UpdateUserAsync(UserDto userDto);
        Task DeleteUserAsync(int id);

        // Product
        Task<IEnumerable<ProductDto>> GetProductsAsync();
        Task UpdateProductAsync(ProductDto productDto);
        Task DeleteProductAsync(int id);

        // Request
        Task<IEnumerable<ProductRequestDto>> GetProductRequestsAsync();
        Task UpdateProductRequestAsync(ProductRequestDto requestDto);
        Task DeleteProductRequestAsync(int id);

        // LimitRule
        Task<IEnumerable<RequesterLimitRuleDto>> GetRequesterLimitRulesAsync();
        Task CreateRequesterLimitRuleAsync(RequesterLimitRuleDto limitRuleDto);
        Task UpdateRequesterLimitRuleAsync(RequesterLimitRuleDto limitRuleDto);
        Task DeleteRequesterLimitRuleAsync(int id);
    }
}
