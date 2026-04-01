using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
            var result = await _productRequestService.ClaimProductAsync(productId, userId);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
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

        // Donor lezárja az igénylést: ?success=true → sikeres átadás, ?success=false → sikertelen
        [Authorize]
        [HttpPost("complete/{requestId}")]
        public async Task<IActionResult> Complete(int requestId, [FromQuery] bool success)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _productRequestService.CompleteRequestAsync(requestId, userId, success);
            return result ? Ok() : BadRequest("Nem sikerült lezárni az igénylést.");
        }

        // Lekérdezi, hogy az adott termékre van-e aktív igénylés a bejelentkezett usertől.
        [Authorize]
        [HttpGet("active-for-product/{productId}")]
        public async Task<IActionResult> GetActiveForProduct(int productId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var requestId = await _productRequestService.GetActiveRequestIdForProductAsync(productId, userId);

            if (requestId.HasValue)
                return Ok(new { requestId = requestId.Value });

            return NotFound();
        }

        // Lekérdezi, hogy az adott termékre van-e aktív igénylés BÁRKI által.
        // 200 OK = foglalt, 404 = szabad
        [Authorize]
        [HttpGet("is-claimed/{productId}")]
        public async Task<IActionResult> IsClaimed(int productId)
        {
            var isClaimed = await _productRequestService.IsProductClaimedAsync(productId);
            return isClaimed ? Ok() : NotFound();
        }
    }
}