using Application.DTOs;

//using Microsoft.AspNetCore.Components.Forms;

namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<bool> CreateProductAsync(ProductDto productDto, int userId);
        Task<IEnumerable<ProductDto>> GetProductsAsync();
        Task DeleteProductAsync(int id);

        Task<bool> ClaimProductAsync(int productId, int requestId);
    }
}
