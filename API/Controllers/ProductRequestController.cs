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

        /// <summary>
        /// Igénylést hoz létre az adott termékre a bejelentkezett felhasználó számára.
        /// </summary>
        /// <param name="productId">A termék azonosítója.</param>
        /// <returns>200 OK siker esetén, ellenkező esetben 400 BadRequest.</returns>
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

        /// <summary>
        /// Lekéri a bejelentkezett felhasználó saját igényléseit.
        /// </summary>
        /// <returns>200 OK a felhasználó igényléseivel.</returns>
        [Authorize]
        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Ok(await _productRequestService.GetMyRequestsAsync(userId));
        }

        /// <summary>
        /// Törli a bejelentkezett felhasználó egyik igénylését.
        /// </summary>
        /// <param name="requestId">Az igénylés azonosítója.</param>
        /// <returns>200 OK siker esetén, 403 Forbidden, ha a művelet nem engedélyezett.</returns>
        [Authorize]
        [HttpDelete("request/{requestId}")]
        public async Task<IActionResult> DeleteRequest(int requestId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var success = await _productRequestService.DeleteRequestAsync(requestId, userId);
            return success ? Ok() : Forbid();
        }

        /// <summary>
        /// Lekéri a bejelentkezett felhasználó termékeihez tartozó igényléseket.
        /// </summary>
        /// <returns>200 OK a beérkezett igénylések listájával.</returns>
        [Authorize]
        [HttpGet("Sender-requests")]
        public async Task<IActionResult> GetSenderRequests()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Ok(await _productRequestService.GetSenderRequestsAsync(userId));
        }

        /// <summary>
        /// Lezárja az adott igénylést.
        /// </summary>
        /// <param name="requestId">Az igénylés azonosítója.</param>
        /// <param name="success">True esetén sikeres átadás, false esetén sikertelen lezárás történik.</param>
        /// <returns>200 OK siker esetén, ellenkező esetben 400 BadRequest.</returns>
        [Authorize]
        [HttpPost("complete/{requestId}")]
        public async Task<IActionResult> Complete(int requestId, [FromQuery] bool success)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _productRequestService.CompleteRequestAsync(requestId, userId, success);
            return result ? Ok() : BadRequest("Nem sikerült lezárni az igénylést.");
        }

        /// <summary>
        /// Lekéri, hogy a bejelentkezett felhasználónak van-e aktív igénylése az adott termékre.
        /// </summary>
        /// <param name="productId">A termék azonosítója.</param>
        /// <returns>200 OK az aktív igénylés azonosítójával, vagy 404 NotFound, ha nincs aktív igénylés.</returns>
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

        /// <summary>
        /// Megvizsgálja, hogy az adott termékre van-e aktív igénylés bármely felhasználó által.
        /// </summary>
        /// <param name="productId">A termék azonosítója.</param>
        /// <returns>200 OK, ha a termék foglalt; 404 NotFound, ha nincs aktív igénylés.</returns>
        [Authorize]
        [HttpGet("is-claimed/{productId}")]
        public async Task<IActionResult> IsClaimed(int productId)
        {
            var isClaimed = await _productRequestService.IsProductClaimedAsync(productId);
            return isClaimed ? Ok() : NotFound();
        }
    }
}
