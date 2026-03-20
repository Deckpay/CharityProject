using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateProductAsync(ProductDto productDto, int userId)
        {
            var newProduct = new Product
            {
                ProductName = productDto.ProductName,
                ProductDescription = productDto.ProductDescription,
                ProductCategoryId = productDto.ProductCategoryId,
                ImagePath = productDto.ImagePath,
                CountyId = productDto.CountyId,
                CreatedAt = DateTime.Now,
                IsActive = true,
                ProductStatus = Domain.Enums.DonationStatus.Active,
                DonorId = userId
            };

            await _unitOfWork.Products.AddAsync(newProduct);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var allProducts = await _unitOfWork.Products.GetAllAsync();
            return allProducts.Where(p => p.IsActive).Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription,
                ImagePath = p.ImagePath,
                ProductCategoryId = p.ProductCategoryId,
                CountyId = p.CountyId,
                DonorId = p.DonorId,
                IsActive = p.IsActive
            }).ToList();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product != null)
            {
                product.IsActive = false;
                product.UpdatedAt = DateTime.Now;
                _unitOfWork.Products.Update(product);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<bool> ClaimProductAsync(int productId, int userId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null) return false;

            if (product.DonorId == userId) return false;

            var allRequests = await _unitOfWork.ProductRequests.GetAllAsync();
            var existing = allRequests.FirstOrDefault(r =>
                r.ProductId == productId && r.RequesterId == userId && r.IsActive);
            if (existing != null) return true;

            var newRequest = new ProductRequest
            {
                ProductId = productId,
                RequesterId = userId,
                RequestStatus = 0,
                IsActive = true,
                RequestedAt = DateTime.Now
            };

            await _unitOfWork.ProductRequests.AddAsync(newRequest);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<IEnumerable<ProductRequestDto>> GetMyRequestsAsync(int userId)
        {
            var allRequests = await _unitOfWork.ProductRequests.GetAllAsync();
            return allRequests.Where(r => r.RequesterId == userId).Select(r => new ProductRequestDto
            {
                ProductRequestId = r.ProductRequestId, // javítva: r.RequesterId volt!
                ProductId = r.ProductId,
                RequesterId = r.RequesterId,
                RequestStatus = r.RequestStatus,
                IsActive = r.IsActive,
                RequestedAt = r.RequestedAt
            });
        }
    }
}