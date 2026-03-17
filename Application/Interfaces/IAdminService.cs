using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task BanUserAsync(int id);

        Task DeleteUserAsync(int id);
        Task UpdateUserAsync(UserDto userDto);

        Task<IEnumerable<ProductDto>> GetProductsAsync();
    }
}
