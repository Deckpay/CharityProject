using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services
{
    /// <summary>
    /// A termékigénylések kezeléséért felelős szolgáltatás.
    /// Kezeli az igénylés létrehozását, lekérdezését, törlését és lezárását.
    /// </summary>
    public class ProductRequestService : IProductRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILimitService _limitService;

        public ProductRequestService(IUnitOfWork unitOfWork, ILimitService limitService)
        {
            _unitOfWork = unitOfWork;
            _limitService = limitService;
        }

        public async Task<ClaimResultDto> ClaimProductAsync(int productId, int userId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);

                if (product == null)
                    return new ClaimResultDto { Success = false, Message = "A termék nem található." };

                if (product.SenderId == userId)
                    return new ClaimResultDto { Success = false, Message = "Saját termékedet nem igényelheted." };

                if (product.ProductStatus != ProductStatus.Active)
                    return new ClaimResultDto { Success = false, Message = "A termék nem igényelhető." };

                var canRequest = await _limitService.CanUserRequestProduct(userId, product.ProductCategoryId);
                if (!canRequest)
                    return new ClaimResultDto { Success = false, Message = "Elérted az igénylési limitet erre a periódusra." };

                bool consumed = await _limitService.UpdateLimitUsage(userId, product.ProductCategoryId);
                if (!consumed)
                    return new ClaimResultDto { Success = false, Message = "Elérted az igénylési limitet." };

                var allRequests = await _unitOfWork.ProductRequests.GetAllAsync();
                if (allRequests.Any(r => r.ProductId == productId && r.RequestStatus == RequestStatus.Pending))
                    return new ClaimResultDto { Success = false, Message = "A termék már foglalt." };

                var newRequest = new ProductRequest
                {
                    ProductId = productId,
                    RequesterId = userId,
                    RequestStatus = RequestStatus.Pending,
                    RequestedAt = DateTime.UtcNow
                };

                product.ProductStatus = ProductStatus.Pending;
                product.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.ProductRequests.AddAsync(newRequest);
                await _unitOfWork.CompleteAsync();
                return new ClaimResultDto { Success = true, RequestId = newRequest.ProductRequestId, Message = "Sikeres igénylés!" };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<IEnumerable<ProductRequestDto>> GetMyRequestsAsync(int userId)
        {
            var requests = await _unitOfWork.ProductRequests.GetAllAsync();
            var dtos = new List<ProductRequestDto>();

            var activeRequests = requests
                .Where(r => r.RequesterId == userId && r.RequestStatus == RequestStatus.Pending)
                .ToList();

            foreach (var r in activeRequests)
            {
                dtos.Add(await MapToDtoAsync(r));
            }

            return dtos;
        }

        public async Task<bool> DeleteRequestAsync(int requestId, int userId)
        {
            var request = await _unitOfWork.ProductRequests.GetByIdAsync(requestId);
            if (request == null || request.RequesterId != userId) return false;

            request.RequestStatus = RequestStatus.Failed;
            request.ProcessedAt = DateTime.UtcNow;

            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
            if (product != null)
            {
                product.ProductStatus = ProductStatus.Active;
                product.UpdatedAt = DateTime.UtcNow;
                await _limitService.DecreaseLimitUsage(request.RequesterId, product.ProductCategoryId);
            }

            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<IEnumerable<ProductRequestDto>> GetSenderRequestsAsync(int userId)
        {
            var myProducts = await _unitOfWork.Products.GetAllAsync();
            var myProductIds = myProducts
                .Where(p => p.SenderId == userId &&
                    (p.ProductStatus == ProductStatus.Active || p.ProductStatus == ProductStatus.Pending))
                .Select(p => p.ProductId)
                .ToHashSet();

            var requests = await _unitOfWork.ProductRequests.GetAllAsync();
            var filteredRequests = requests
                .Where(r => myProductIds.Contains(r.ProductId) && r.RequestStatus == RequestStatus.Pending)
                .ToList();

            var dtos = new List<ProductRequestDto>();
            foreach (var r in filteredRequests)
            {
                dtos.Add(await MapToDtoAsync(r));
            }

            return dtos;
        }
        public async Task<bool> CompleteRequestAsync(int requestId, int userId, bool success)
        {
            var request = await _unitOfWork.ProductRequests.GetByIdAsync(requestId);
            if (request == null) return false;

            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
            if (product == null || product.SenderId != userId) return false;

            if (success)
            {
                request.RequestStatus = RequestStatus.Completed;
                request.ProcessedAt = DateTime.UtcNow;
                product.ProductStatus = ProductStatus.Completed;
                product.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                request.RequestStatus = RequestStatus.Failed;
                request.ProcessedAt = DateTime.UtcNow;
                product.ProductStatus = ProductStatus.Active;
                product.UpdatedAt = DateTime.UtcNow;
                await _limitService.DecreaseLimitUsage(request.RequesterId, product.ProductCategoryId);
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

        private async Task<ProductRequestDto> MapToDtoAsync(ProductRequest r)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(r.ProductId);
            return new ProductRequestDto
            {
                ProductRequestId = r.ProductRequestId,
                ProductId = r.ProductId,
                ProductName = product?.ProductName ?? $"Termék #{r.ProductId}",
                RequesterId = r.RequesterId,
                RequestStatus = r.RequestStatus,
                RequestedAt = r.RequestedAt
            };
        }
    }
}
