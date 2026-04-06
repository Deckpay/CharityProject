using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ILimitService
    {
        Task<bool> CanUserRequestProduct(int userId, int categoryId);
        Task<bool> UpdateLimitUsage(int userId, int categoryId);
        Task<bool> DecreaseLimitUsage(int userId, int categoryId);
    }
}
