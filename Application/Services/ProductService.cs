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

            // A GenericRepository "AddAsync" metódusát hívjuk meg:
            await _unitOfWork.Products.AddAsync(newProduct);

            // Végül a UnitOfWork-kel véglegesítjük az összes változtatást az adatbázisban
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            // softdel miatt csak az aktív termékeket adja vissza
            var allProducts = await _unitOfWork.Products.GetAllAsync();

            return allProducts.Where(p => p.IsActive).Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription,
                ImagePath= p.ImagePath,
                ProductCategoryId = p.ProductCategoryId,
                CountyId = p.CountyId,
                DonorId = p.DonorId,
                IsActive = p.IsActive
            }).ToList();
        }
        public async Task DeleteProductAsync(int id)
        {
            // softdelete
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product != null)
            {
                product.IsActive = false;
                product.UpdatedAt = DateTime.Now;

                _unitOfWork.Products.Update(product);

                await _unitOfWork.CompleteAsync();
            }
        }

        // igénylés
        public async Task<bool> ClaimProductAsync(int productId, int userId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null) return false;

            //ellenörzés ne igényeljen saját terméket
            if (product.DonorId == userId) return false;
            

            var newRequest = new ProductRequest
            {
                //ProductRequestId = requestId,
                //DonorId = product.DonorId,
                ProductId = productId,                
                RequesterId = userId,
                RequestStatus = 0,
                IsActive = true,
                RequestedAt = DateTime.Now
                
               
            };

            await _unitOfWork.ProductRequests.AddAsync(newRequest);
            

           
                // itt hivjuk meg a chat servict hogy keszitse el az uj rekordokat
            return await  _unitOfWork.CompleteAsync() > 0;

        }

        public async Task<IEnumerable<ProductRequestDto>> GetMyRequestsAsync(int userId)
        {
            var allRequests = await _unitOfWork.ProductRequests.GetAllAsync();
            return allRequests.Where(r => r.RequesterId == userId).Select(r => new ProductRequestDto 
            {
                ProductRequestId = r.RequesterId,
                ProductId = r.ProductId,
               RequesterId = r.RequesterId,
               RequestStatus = r.RequestStatus,
               IsActive = r.IsActive,
               RequestedAt = r.RequestedAt
            });
                
        }
    }
}
