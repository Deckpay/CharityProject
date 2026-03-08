using Application.DTOs;

//using Microsoft.AspNetCore.Components.Forms;

namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<bool> CreateProductAsync(ProductDto productDto);
        Task<IEnumerable<ProductDto>> GetProductsAsync();
        Task DeleteProductAsync(int id);
    }
}
