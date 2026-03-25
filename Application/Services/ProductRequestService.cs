using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (product == null || product.DonorId == userId) return 0;

            var allRequests = await _unitOfWork.ProductRequests.GetAllAsync();
            var existing = allRequests.FirstOrDefault(r => r.ProductId == productId && r.RequesterId == userId && r.RequestStatus == (int)RequestStatus.Pending);

            if (existing != null) return existing.ProductRequestId;

            var newRequest = new ProductRequest
            {
                ProductId = productId,
                RequesterId = userId,
                RequestStatus = (int)RequestStatus.Pending,
                RequestedAt = DateTime.Now
            };

            await _unitOfWork.ProductRequests.AddAsync(newRequest);
            await _unitOfWork.CompleteAsync();

            return newRequest.ProductRequestId; // Visszaadjuk a generált ID-t
        }

        public async Task<IEnumerable<ProductRequestDto>> GetMyRequestsAsync(int userId)
        {
            var requests = await _unitOfWork.ProductRequests.GetAllAsync();
            return requests.Where(r => r.RequesterId == userId && r.RequestStatus == (int)RequestStatus.Pending)
                           .Select(MapToRequestDto);
        }

        public async Task<bool> DeleteRequestAsync(int requestId, int userId)
        {
            var request = await _unitOfWork.ProductRequests.GetByIdAsync(requestId);
            if (request == null || request.RequesterId != userId) return false;

            request.RequestStatus = (int)RequestStatus.Failed;
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<IEnumerable<ProductRequestDto>> GetDonorRequestsAsync(int userId)
        {
            var myProducts = await _unitOfWork.Products.GetAllAsync();
            var myProductIds = myProducts.Where(p => p.DonorId == userId && p.ProductStatus == ProductStatus.Active)
                                         .Select(p => p.ProductId).ToHashSet();

            var requests = await _unitOfWork.ProductRequests.GetAllAsync();
            return requests.Where(r => myProductIds.Contains(r.ProductId) && r.RequestStatus == (int)RequestStatus.Pending)
                           .Select(MapToRequestDto);
        }

        // Segédmetódusok a mappoláshoz (később használhatsz AutoMappert is)
        

        private static ProductRequestDto MapToRequestDto(ProductRequest r) => new ProductRequestDto
        {
            ProductRequestId = r.ProductRequestId,
            ProductId = r.ProductId,
            RequesterId = r.RequesterId,
            RequestStatus = r.RequestStatus,
            RequestedAt = r.RequestedAt
        };
    }
}
