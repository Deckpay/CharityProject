using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<bool> CreateProductAsync(ProductDto productDto, int userId);
        Task<IEnumerable<ProductDto>> GetProductsAsync();
        Task DeleteProductAsync(int id);
        Task<IEnumerable<ProductRequestDto>> GetMyRequestsAsync(int userId);

        Task<bool> ClaimProductAsync(int productId, int requestId);
    }
}
