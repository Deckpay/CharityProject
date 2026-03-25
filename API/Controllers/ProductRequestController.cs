using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Enums;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductRequestController : ControllerBase
    {
        private readonly IProductRequestService _productRequestService;

        public ProductRequestController(IProductRequestService productRequestService)
        {
            _productRequestService = productRequestService;
        }

        [Authorize]
        [HttpPost("claim/{productId}")]
        public async Task<IActionResult> Claim(int productId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var requestId = await _productRequestService.ClaimProductAsync(productId, userId);

            if (requestId > 0)
                return Ok(new { requestId = requestId }); // JSON válasz: {"requestId": 123}

            return BadRequest("Nem sikerült az igénylés.");
        }

        [Authorize]
        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Ok(await _productRequestService.GetMyRequestsAsync(userId));
        }

        [Authorize]
        [HttpDelete("request/{requestId}")]
        public async Task<IActionResult> DeleteRequest(int requestId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var success = await _productRequestService.DeleteRequestAsync(requestId, userId);
            return success ? Ok() : Forbid();
        }

        [Authorize]
        [HttpGet("donor-requests")]
        public async Task<IActionResult> GetDonorRequests()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Ok(await _productRequestService.GetDonorRequestsAsync(userId));
        }


    }
}
