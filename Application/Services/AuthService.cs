using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

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
                CountyId = registerDto.CountyId, // Itt kasztoljuk vissza az int-et az Enummá:
                UserRole = (Domain.Enums.UserRole)registerDto.RoleId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                CreatedAt = DateTime.Now,
                IsActive = true
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

        public async Task<IEnumerable<County>> GetCountiesAsync()
        {
            return await _unitOfWork.Counties.GetAllAsync();
        }

        public async Task<IEnumerable<ProductCategory>> GetProductCategoriesAsync()
        {
            return await _unitOfWork.Categories.GetAllAsync();
        }
    }
}
