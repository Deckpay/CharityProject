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
            var request = await _unitOfWork.ProductRequests.GetByIdAsync(requestDto.ProductRequestId);

            if (request == null) throw new Exception("Az igénylés nem található");

            request.RequestStatus = requestDto.RequestStatus;

            if (requestDto.RequestStatus == RequestStatus.Failed)
            {
                request.ProcessedAt = DateTime.UtcNow;
            }
            else
            {
                request.ProcessedAt = DateTime.MinValue;
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteProductRequestAsync(int id)
        {
            var request = await _unitOfWork.ProductRequests.GetByIdAsync(id);

            if (request == null)
            {
                return;
            }

            request.RequestStatus = RequestStatus.Failed;
            request.ProcessedAt = DateTime.UtcNow;

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
    }
}
