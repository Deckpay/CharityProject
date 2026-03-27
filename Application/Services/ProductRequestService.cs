using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services
{
    public class ProductRequestService : IProductRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> ClaimProductAsync(int productId, int userId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);

            // Nem igényelheti a saját termékét
            if (product == null || product.DonorId == userId) return 0;

            // Termék már nem Active (pl. Completed, Deleted) → nem igényelhető
            if (product.ProductStatus != ProductStatus.Active) return 0;

            var allRequests = await _unitOfWork.ProductRequests.GetAllAsync();

            // Ha már van BÁRKI által benyújtott aktív (Pending) igénylés erre a termékre
            // → a termék már "foglalt", más nem igényelheti
            var anyActiveClaim = allRequests.Any(r =>
                r.ProductId == productId &&
                r.RequestStatus == RequestStatus.Pending);

            if (anyActiveClaim) return 0;

            // Ha ennek a usernek már volt korábban igénylése (pl. törölt) → új igénylés
            var newRequest = new ProductRequest
            {
                ProductId = productId,
                RequesterId = userId,
                RequestStatus = RequestStatus.Pending,
                RequestedAt = DateTime.Now
            };

            await _unitOfWork.ProductRequests.AddAsync(newRequest);
            await _unitOfWork.CompleteAsync();
            return newRequest.ProductRequestId;
        }

        public async Task<IEnumerable<ProductRequestDto>> GetMyRequestsAsync(int userId)
        {
            var requests = await _unitOfWork.ProductRequests.GetAllAsync();

            // Csak Pending igénylések jelennek meg a rec listájában
            // Ha Completed vagy Failed → eltűnik (a donor lezárta)
            return requests
                .Where(r => r.RequesterId == userId && r.RequestStatus == RequestStatus.Pending)
                .Select(MapToDto);
        }

        public async Task<bool> DeleteRequestAsync(int requestId, int userId)
        {
            var request = await _unitOfWork.ProductRequests.GetByIdAsync(requestId);
            if (request == null || request.RequesterId != userId) return false;

            request.RequestStatus = RequestStatus.Failed;
            request.ProcessedAt = DateTime.UtcNow;

            // Ha a rec visszavonja az igénylést, a termék visszakerül Active-ba
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
            if (product != null && product.ProductStatus == ProductStatus.Active)
            {
                // Már Active, nem kell változtatni
            }

            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<IEnumerable<ProductRequestDto>> GetDonorRequestsAsync(int userId)
        {
            var myProducts = await _unitOfWork.Products.GetAllAsync();
            var myProductIds = myProducts
                .Where(p => p.DonorId == userId && p.ProductStatus == ProductStatus.Active)
                .Select(p => p.ProductId)
                .ToHashSet();

            var requests = await _unitOfWork.ProductRequests.GetAllAsync();
            return requests
                .Where(r => myProductIds.Contains(r.ProductId) && r.RequestStatus == RequestStatus.Pending)
                .Select(MapToDto);
        }

        public async Task<bool> CompleteRequestAsync(int requestId, int userId, bool success)
        {
            var request = await _unitOfWork.ProductRequests.GetByIdAsync(requestId);
            if (request == null) return false;

            // Csak a termék donora zárhatja le
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
            if (product == null || product.DonorId != userId) return false;

            if (success)
            {
                // Sikeres átadás → termék Completed, igénylés Completed
                request.RequestStatus = RequestStatus.Completed;
                request.ProcessedAt = DateTime.UtcNow;
                product.ProductStatus = ProductStatus.Completed;
                product.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Sikertelen átadás → termék visszakerül Active-ba, igénylés Failed
                // Így más rec-ek újra tudják igényelni
                request.RequestStatus = RequestStatus.Failed;
                request.ProcessedAt = DateTime.UtcNow;
                product.ProductStatus = ProductStatus.Active;
                product.UpdatedAt = DateTime.UtcNow;
            }

            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<int?> GetActiveRequestIdForProductAsync(int productId, int userId)
        {
            var allRequests = await _unitOfWork.ProductRequests.GetAllAsync();
            var found = allRequests.FirstOrDefault(r =>
                r.ProductId == productId &&
                r.RequesterId == userId &&
                r.RequestStatus == RequestStatus.Pending);
            return found?.ProductRequestId;
        }

        public async Task<bool> IsProductClaimedAsync(int productId)
        {
            var allRequests = await _unitOfWork.ProductRequests.GetAllAsync();
            return allRequests.Any(r =>
                r.ProductId == productId &&
                r.RequestStatus == RequestStatus.Pending);
        }

        private static ProductRequestDto MapToDto(ProductRequest r) => new ProductRequestDto
        {
            ProductRequestId = r.ProductRequestId,
            ProductId = r.ProductId,
            RequesterId = r.RequesterId,
            RequestStatus = r.RequestStatus,
            RequestedAt = r.RequestedAt
        };
    }
}