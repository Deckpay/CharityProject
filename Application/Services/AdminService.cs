using Application.DTOs;
using Application.Interfaces;
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

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var producst = await _unitOfWork.Products.GetAllAsync();

            return producst.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription,
                ProductSatus = p.ProductStatus,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                DonorId = p.DonorId,
                ProductCategoryId = p.ProductCategoryId,
                CountyId = p.CountyId,
            });
        }
    }
}
