using Application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
        Task<IEnumerable<ProductDto>> GetProductsBySenderAsync(int userId);
        Task<bool> CreateProductAsync(ProductDto productDto, int userId, IFormFile imageFile);
        Task UpdateProductAsync(ProductDto productDto);
        Task DeleteProductAsync(int id);



    }
}
