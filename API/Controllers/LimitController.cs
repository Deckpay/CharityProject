using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LimitController : ControllerBase
    {
        private readonly ILimitService _limitService;

        public LimitController(ILimitService limitService)
        {
            _limitService = limitService;
        }

        // GET: api/limit/can-request?userId=1&categoryId=2
        [HttpGet("can-request")]
        public async Task<IActionResult> CanRequest(int userId, int categoryId)
        {
            var canRequest = await _limitService.CanUserRequestProduct(userId, categoryId);
            return Ok(new { canRequest });
        }

        // POST: api/limit/use
        [HttpPost("use")]
        public async Task<IActionResult> UseLimit(int userId, int categoryId)
        {
            bool success = await _limitService.UpdateLimitUsage(userId, categoryId);
            return Ok(new { success });
        }
    }
}
