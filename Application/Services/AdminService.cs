using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // user
        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();

            return users.Select(u => new UserDto
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                UserRole = u.UserRole,
                UserStatus = u.UserStatus,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
            });
        }
        public async Task BanUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);

            if (user == null)
            {
                return;
            }

            user.UserStatus = UserStatus.Banned;

            await SyncProductsWithUserAsync(user);

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);

            if (user == null)
            {
                return;
            }

            user.UserStatus = UserStatus.Deleted;

            await SyncProductsWithUserAsync(user);

            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateUserAsync(UserDto userDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userDto.UserId);

            if (user == null) throw new Exception("A felhasználó nem található");

            user.UserName = userDto.UserName!;
            user.Email = userDto.Email!;
            user.UserRole = userDto.UserRole;
            user.UserStatus = userDto.UserStatus;
            user.UpdatedAt = DateTime.UtcNow;

            await SyncProductsWithUserAsync(user);

            await _unitOfWork.CompleteAsync();
        }

        // product
        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var producst = await _unitOfWork.Products.GetAllAsync();

            return producst.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription,
                ProductStatus = p.ProductStatus,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                SenderId = p.SenderId,
                ProductCategoryId = p.ProductCategoryId,
                CountyId = p.CountyId,
            });
        }

        public async Task UpdateProductAsync(ProductDto productDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productDto.ProductId);

            if (product == null) throw new Exception("A termék nem található");

            product.ProductName = productDto.ProductName;
            product.ProductDescription = productDto.ProductDescription;
            product.ProductStatus = productDto.ProductStatus;
            product.CountyId = productDto.CountyId;
            product.ProductCategoryId = productDto.ProductCategoryId;
            product.UpdatedAt = DateTime.UtcNow;

            await SyncRequestWithProductAsync(product);

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
            {
                return;
            }

            product.ProductStatus = ProductStatus.Deleted;

            await SyncRequestWithProductAsync(product);

            await _unitOfWork.CompleteAsync();
        }

        // request
        public async Task<IEnumerable<ProductRequestDto>> GetProductRequestsAsync()
        {
            var requests = await _unitOfWork.ProductRequests.GetAllAsync();

            if (requests == null) throw new Exception("Az igénylés nem található");

            return requests.Select(p => new ProductRequestDto
            {
                ProductRequestId = p.ProductRequestId,
                ProductId = p.ProductId,
                RequesterId = p.RequesterId,
                RequestStatus = p.RequestStatus,
                RequestedAt = p.RequestedAt,
                ProcessedAt = p.ProcessedAt ?? DateTime.MinValue
            });
        }

        public async Task UpdateProductRequestAsync(ProductRequestDto requestDto)
        {
            // igénylés betöltése
            var request = await _unitOfWork.ProductRequests.GetByIdAsync(requestDto.ProductRequestId);
            if (request == null) throw new Exception("Az igénylés nem található");

            // kapcsolódó termék betöltése
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);

            switch (requestDto.RequestStatus)
            {
                case RequestStatus.Pending:
                    request.RequestStatus = RequestStatus.Pending;
                    request.ProcessedAt = null;

                    if (product != null)
                        product.ProductStatus = ProductStatus.Pending;

                    break;

                case RequestStatus.Failed:
                    request.RequestStatus = RequestStatus.Failed;
                    request.ProcessedAt = DateTime.UtcNow;

                    if (product != null)
                        product.ProductStatus = ProductStatus.Active;

                    break;

                default:
                    request.RequestStatus = requestDto.RequestStatus;
                    request.ProcessedAt = DateTime.UtcNow;

                    if (product != null)
                        product.ProductStatus = ProductStatus.Completed;

                    break;
            }

            if (product != null)
                product.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteProductRequestAsync(int id)
        {
            var request = await _unitOfWork.ProductRequests.GetByIdAsync(id);

            if (request == null)
            {
                return;
            }

            // kapcsolódó termék betöltése
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);

            request.RequestStatus = RequestStatus.Failed;
            request.ProcessedAt = DateTime.UtcNow;

            if (product != null)
            {
                product.ProductStatus = ProductStatus.Active;
                product.UpdatedAt = DateTime.UtcNow;
            }

            await _unitOfWork.CompleteAsync();
        }

        // limit rule
        public async Task<IEnumerable<RequesterLimitRuleDto>> GetRequesterLimitRules()
        {
            var limitRules = await _unitOfWork.RequesterLimitRules.GetAllAsync();

            if (limitRules == null) throw new Exception("A szabály nem található");

            return limitRules.Select(r => new RequesterLimitRuleDto
            {
                RequesterLimitRuleId = r.RequesterLimitRuleId,
                RequesterLimitRuleCategoryId = r.RequesterLimitRuleCategoryId,
                PeriodType = r.PeriodType,
                MaxQuantity = r.MaxQuantity,
                RequesterLimitRuleDescription = r.RequesterLimitRuleDescription,
                IsActive = r.IsActive,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            });
        }

        public async Task CreateRequesterLimitRule(RequesterLimitRuleDto limitRuleDto)
        {
            var limitRule = new RequesterLimitRule
            {
                RequesterLimitRuleCategoryId = limitRuleDto.RequesterLimitRuleCategoryId,
                PeriodType = limitRuleDto.PeriodType,
                MaxQuantity = limitRuleDto.MaxQuantity,
                RequesterLimitRuleDescription = limitRuleDto.RequesterLimitRuleDescription,
                IsActive = limitRuleDto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.RequesterLimitRules.AddAsync(limitRule);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateRequesterLimitRule(RequesterLimitRuleDto limitRuleDto)
        {
            var limitRule = await _unitOfWork.RequesterLimitRules.GetByIdAsync(limitRuleDto.RequesterLimitRuleId);

            if (limitRule == null) throw new Exception("Az igénylés nem található");

            limitRule.RequesterLimitRuleCategoryId = limitRuleDto.RequesterLimitRuleCategoryId;
            limitRule.RequesterLimitRuleDescription = limitRuleDto.RequesterLimitRuleDescription;
            limitRule.MaxQuantity = limitRuleDto.MaxQuantity;
            limitRule.PeriodType = limitRuleDto.PeriodType;
            limitRule.IsActive = limitRuleDto.IsActive;
            limitRule.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteRequesterLimitRule(int id)
        {
            var limitRule = await _unitOfWork.RequesterLimitRules.GetByIdAsync(id);

            if (limitRule == null)
            {
                return;
            }

            limitRule.IsActive = false;

            await _unitOfWork.CompleteAsync();
        }

        private async Task SyncRequestWithProductAsync(Product product)
        {
            var allRequests = await _unitOfWork.ProductRequests.GetAllAsync();
            var request = allRequests.FirstOrDefault(r => r.ProductId == product.ProductId);

            if (request == null)
                return;

            switch (product.ProductStatus)
            {
                case ProductStatus.Pending:
                    request.RequestStatus = RequestStatus.Pending;
                    request.ProcessedAt = null;
                    break;

                case ProductStatus.Completed:
                    request.RequestStatus = RequestStatus.Completed;
                    request.ProcessedAt = DateTime.UtcNow;
                    break;

                case ProductStatus.Deleted:
                    request.RequestStatus = RequestStatus.Failed; // vagy Deleted, ha van ilyen enum
                    request.ProcessedAt = DateTime.UtcNow;
                    break;

                case ProductStatus.Active:
                    // csak akkor állítsd vissza, ha van értelme
                    if (request.RequestStatus == RequestStatus.Pending)
                    {
                        request.RequestStatus = RequestStatus.Failed;
                        request.ProcessedAt = DateTime.UtcNow;
                    }
                    break;
            }
        }
        private async Task SyncProductsWithUserAsync(User user)
        {
            var allProducts = await _unitOfWork.Products.GetAllAsync();
            var products = allProducts.Where(p => p.SenderId == user.UserId).ToList();

            if (products == null)
                return;

            foreach (var product in products)
            {
                switch (user.UserStatus)
                {
                    case UserStatus.Banned:
                    case UserStatus.Deleted:
                        product.ProductStatus = ProductStatus.Deleted;
                        product.UpdatedAt = DateTime.UtcNow;
                        await SyncRequestWithProductAsync(product);
                        break;
                }
            }
        }
    }
}
