using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services
{
    /// <summary>
    /// Hitelesítési és felhasználói fiókkezelési műveletekért felelős szolgáltatás.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public AuthService(IUnitOfWork unitOfWork, IJwtTokenGenerator jwtTokenGenerator)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            var existingUsers = await _unitOfWork.Users.FindAsync(u => 
                u.Email == registerDto.Email || u.UserName == registerDto.UserName);

            if (existingUsers.Any())
                return false;

            var newUser = new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserRole = (Domain.Enums.UserRole)registerDto.RoleId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                CreatedAt = DateTime.UtcNow,
                UserStatus = UserStatus.Active
            };

            await _unitOfWork.Users.AddAsync(newUser);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<LoginResponseDto?> LoginAsync(string emailOrUserName, string password)
        {
            var user = (await _unitOfWork.Users.FindAsync(u => u.Email == emailOrUserName || u.UserName == emailOrUserName)).FirstOrDefault();

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return new LoginResponseDto { ErrorMessage = "Hibás e-mail cím vagy jelszó." };

            if (user.UserStatus == UserStatus.Banned)
                return new LoginResponseDto { ErrorMessage = "Ez a fiók le van tiltva." };

            if (user.UserStatus == UserStatus.Deleted)
                return new LoginResponseDto { ErrorMessage = "Ez a fiók törölve lett." };

            var token = _jwtTokenGenerator.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token
            };
        }

        public async Task<bool> DeleteMyAccountAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
                return false;
            
            user.UserStatus = UserStatus.Deleted;
            user.UpdatedAt = DateTime.UtcNow;

            var userProducts = await _unitOfWork.Products.FindAsync(p => p.SenderId == userId);

            var userRequests = await _unitOfWork.ProductRequests.FindAsync(r => r.RequesterId == userId);

            if (userProducts.Any())
            {
                foreach (var product in userProducts)
                {
                    product.ProductStatus = ProductStatus.Deleted;
                    product.UpdatedAt = DateTime.UtcNow;

                    var relatedRequests = await _unitOfWork.ProductRequests.FindAsync(
                        r => r.ProductId == product.ProductId && r.RequestStatus == RequestStatus.Pending);

                    foreach (var request in relatedRequests)
                    {
                        request.RequestStatus = RequestStatus.Failed;
                        request.ProcessedAt = DateTime.UtcNow;
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

                    var relatedProduct = await _unitOfWork.Products.GetByIdAsync(request.ProductId);

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
