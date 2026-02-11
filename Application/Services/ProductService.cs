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

        public async Task<bool> CreateProductAsync(ProductDto productDto)
        {
            var newProduct = new Product
            {
                ProductName = productDto.ProductName,
                ProductDescription = productDto.ProductDescription,
                ProductCategoryId = productDto.ProductCategoryId,
                CountyId = productDto.CountyId,
                CreatedAt = DateTime.Now,
                IsActive = true,
                ProductStatus = Domain.Enums.DonationStatus.Active,
                DonorId = 1
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
                ProductCategoryId = p.ProductCategoryId,
                CountyId = p.CountyId
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
    }
}
