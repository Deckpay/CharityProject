using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    /// <summary>
    /// A termékek kezeléséért felelős szolgáltatás.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return products.Where(p => p.ProductStatus == ProductStatus.Active)
                           .Select(MapToDto);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsBySenderAsync(int userId)
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return products.Where(p => p.SenderId == userId 
                && p.ProductStatus != ProductStatus.Deleted
                && p.ProductStatus != ProductStatus.Completed)
                           .Select(MapToDto);
        }

        public async Task<bool> CreateProductAsync(ProductDto dto, int userId, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");

                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                dto.ImagePath = "/images/products/" + fileName;
            }

            var newProduct = new Product
            {
                ProductName = dto.ProductName,
                ProductDescription = dto.ProductDescription,
                ProductCategoryId = dto.ProductCategoryId,
                ImagePath = dto.ImagePath,
                CountyId = dto.CountyId,
                CreatedAt = DateTime.UtcNow,
                ProductStatus = ProductStatus.Active,
                SenderId = userId
            };

            await _unitOfWork.Products.AddAsync(newProduct);
            return await _unitOfWork.CompleteAsync() > 0;
        }
        public async Task UpdateProductAsync(ProductDto productDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productDto.ProductId);

            if (product == null) throw new Exception("A termék nem található");

            if (product.ProductStatus != ProductStatus.Active)
                throw new Exception("Csak aktív termék szerkeszthető");

            product.ProductName = productDto.ProductName;
            product.ProductDescription = productDto.ProductDescription;
            product.CountyId = productDto.CountyId;
            product.ProductCategoryId = productDto.ProductCategoryId;
            product.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CompleteAsync();
        }
        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product != null)
            {
                product.ProductStatus = ProductStatus.Deleted;
                product.UpdatedAt = DateTime.Now;
                _unitOfWork.Products.Update(product);
                await _unitOfWork.CompleteAsync();
            }
        }

        private static ProductDto MapToDto(Product p) => new ProductDto
        {
            ProductId = p.ProductId,
            ProductName = p.ProductName,
            ProductDescription = p.ProductDescription,
            ImagePath = p.ImagePath,
            ProductCategoryId = p.ProductCategoryId,
            CountyId = p.CountyId,
            SenderId = p.SenderId,
            ProductStatus = p.ProductStatus
        };
    }
}
