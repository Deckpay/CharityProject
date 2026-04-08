using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            // 1. Ellenőrizzük, létezik-e már a felhasználó
            var existingUsers = await _unitOfWork.Users.GetAllAsync();
            if (existingUsers.Any(u => u.Email == registerDto.Email || u.UserName == registerDto.UserName))
            {
                return false;
            }

            // 2. Felhasználó létrehozása és jelszó titkosítás
            var newUser = new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserRole = (Domain.Enums.UserRole)registerDto.RoleId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                CreatedAt = DateTime.Now,
                UserStatus = UserStatus.Active
            };

            await _unitOfWork.Users.AddAsync(newUser);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<User?> LoginAsync(string emailOrUserName, string password)
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Email == emailOrUserName || u.UserName == emailOrUserName);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null; // Hibás jelszó vagy felhasznláló
            }

            return user;
        }

        public async Task<bool> DeleteMyAccountAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
                return false;
            
            user.UserStatus = UserStatus.Deleted;
            user.UpdatedAt = DateTime.UtcNow;

            var allPorducts = await _unitOfWork.Products.GetAllAsync();
            var userProducts = allPorducts.Where(p => p.SenderId == userId).ToList();

            var allRequests = await _unitOfWork.ProductRequests.GetAllAsync();
            var userRequests = allRequests.Where(r => r.RequesterId == userId).ToList();

            if (userProducts.Any())
            {
                foreach (var product in userProducts)
                {
                    product.ProductStatus = ProductStatus.Deleted;
                    product.UpdatedAt = DateTime.UtcNow;

                    var relatedRequests = allRequests
                        .Where(r => r.ProductId == product.ProductId)
                        .ToList();

                    foreach (var request in relatedRequests)
                    {
                        if (request.RequestStatus == RequestStatus.Pending)
                        {
                            request.RequestStatus = RequestStatus.Failed;
                            request.ProcessedAt = DateTime.UtcNow;
                        }
                    }
                }
            }

            if (userRequests.Any())
            {
                foreach (var request in userRequests)
                {
                    if (request.RequestStatus != RequestStatus.Pending)
                        continue;

                    request.RequestStatus = RequestStatus.Failed;
                    request.ProcessedAt = DateTime.UtcNow;

                    var relatedProduct = allPorducts.FirstOrDefault(p => p.ProductId == request.ProductId);

                    if (relatedProduct != null && relatedProduct.ProductStatus == ProductStatus.Pending)
                    {
                        relatedProduct.ProductStatus = ProductStatus.Active;
                        relatedProduct.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }

            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
                return false;

            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
                return false;

            if (changePasswordDto.CurrentPassword == changePasswordDto.NewPassword)
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}
