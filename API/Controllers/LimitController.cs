using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize (Roles = "Admin")]
    public class LimitController : ControllerBase
    {
        private readonly ILimitService _limitService;

        public LimitController(ILimitService limitService)
        {
            _limitService = limitService;
        }

        /// <summary>
        /// Megvizsgálja, hogy a felhasználó küldhet-e új igénylést az adott kategóriában.
        /// </summary>
        /// <param name="userId">A felhasználó azonosítója.</param>
        /// <param name="categoryId">A kategória azonosítója.</param>
        /// <returns>True, ha a kérés engedélyezett; különben false.</returns>
        [HttpGet("can-request")]
        public async Task<IActionResult> CanRequest(int userId, int categoryId)
        {
            var canRequest = await _limitService.CanUserRequestProduct(userId, categoryId);
            return Ok(new { canRequest });
        }

        /// <summary>
        /// Frissíti a felhasználó limit használatát (egy új igénylés után).
        /// </summary>
        /// <param name="userId">A felhasználó azonosítója.</param>
        /// <param name="categoryId">A kategória azonosítója.</param>
        /// <returns>True, ha a frissítés sikeres.</returns>
        [HttpPost("use")]
        public async Task<IActionResult> UseLimit(int userId, int categoryId)
        {
            bool success = await _limitService.UpdateLimitUsage(userId, categoryId);
            return Ok(new { success });
        }
    }
}
