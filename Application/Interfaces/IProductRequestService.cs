using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IProductRequestService
    {
        Task<int> ClaimProductAsync(int productId, int userId);
        Task<IEnumerable<ProductRequestDto>> GetMyRequestsAsync(int userId);
        Task<bool> DeleteRequestAsync(int requestId, int userId);
        Task<IEnumerable<ProductRequestDto>> GetDonorRequestsAsync(int userId);


    }
}
