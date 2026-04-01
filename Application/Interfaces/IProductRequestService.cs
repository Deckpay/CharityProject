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
        Task<ClaimResultDto> ClaimProductAsync(int productId, int userId);
        Task<IEnumerable<ProductRequestDto>> GetMyRequestsAsync(int userId);
        Task<bool> DeleteRequestAsync(int requestId, int userId);
        Task<IEnumerable<ProductRequestDto>> GetDonorRequestsAsync(int userId);
        Task<bool> CompleteRequestAsync(int requestId, int userId, bool success);
        Task<int?> GetActiveRequestIdForProductAsync(int productId, int userId);
        Task<bool> IsProductClaimedAsync(int productId);


    }
}
