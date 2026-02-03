using Domain.Entities;

namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<bool> CreateProductAsync(ProductDto productDto);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task DeleteProductAsync(int id);
    }
}
