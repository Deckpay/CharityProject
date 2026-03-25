using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAdminService
    {
        // User actions
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task BanUserAsync(int id);
        Task UpdateUserAsync(UserDto userDto);
        Task DeleteUserAsync(int id);

        // Product actions
        Task<IEnumerable<ProductDto>> GetProductsAsync();
        Task UpdateProductAsync(ProductDto productDto);
        Task DeleteProductAsync(int id);

        // Request actions
        Task<IEnumerable<ProductRequestDto>> GetProductRequestsAsync();
        Task UpdateProductRequestAsync(ProductRequestDto requestDto);
        Task DeleteProductRequestAsync(int id);

        // LimitRule actions
        Task<IEnumerable<RequesterLimitRuleDto>> GetRequesterLimitRules();
        Task CreateRequesterLimitRule(RequesterLimitRuleDto limitRuleDto);
        Task UpdateRequesterLimitRule(RequesterLimitRuleDto limitRuleDto);
        Task DeleteRequesterLimitRule(int id);
    }
}
