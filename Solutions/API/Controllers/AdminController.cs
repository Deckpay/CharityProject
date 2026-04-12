using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>
        /// Lekéri az összes felhasználót admin számára.
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _adminService.GetUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Felhasználó tiltása (soft-ban).
        /// Nem törli az adatbázisból, csak deaktiválja.
        /// </summary>
        /// <param name="id">Felhasználó azonosítója</param>
        [HttpPut("ban-user/{id}")]
        public async Task<IActionResult> BanUser(int id)
        {
            try
            {
                await _adminService.BanUserAsync(id);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Felhasználó törlése (soft-del).
        /// Nem törli az adatbázisból, csak deaktiválja.
        /// </summary>
        /// <param name="id">Felhasználó azonosítója</param>        
        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _adminService.DeleteUserAsync(id);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Felhasználó adatainak frissítése.
        /// </summary>
        /// <param name="userDto">A DTO tartalmazza az ID-t és a módosítandó mezőket.</param>        
        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser(UserDto userDto)
        {
            await _adminService.UpdateUserAsync(userDto);
            return Ok();
        }

        /// <summary>
        /// Lekérdezi az összes terméket.
        /// </summary>        
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _adminService.GetProductsAsync();
            return Ok(products);
        }

        /// <summary>
        /// Frissítí a termék adatait.
        /// </summary>
        /// <param name="productDto">A dto tartalmazza a modosítandó mezőket és az id-t</param>        
        [HttpPut("update-product")]
        public async Task<IActionResult> UpdateProducts(ProductDto productDto)
        {
            await _adminService.UpdateProductAsync(productDto);
            return Ok();
        }

        /// <summary>
        /// Törli a terméket (soft-del).
        /// </summary>
        /// <param name="id">Termék azonosítója.</param>        
        [HttpDelete("delete-product/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _adminService.DeleteProductAsync(id);
            return Ok();
        }

        /// <summary>
        /// Lekéri az összes igénylést.
        /// </summary>        
        [HttpGet("product-requests")]
        public async Task<IActionResult> GetProductRequests()
        {
            var requests = await _adminService.GetProductRequestsAsync();
            return Ok(requests);
        }

        /// <summary>
        /// Frissíti az igénylést.
        /// </summary>
        /// <param name="requestDto">A dto az igénylés modosítandó paramétereit tartalmazza.</param>        
        [HttpPut("update-product-requests")]
        public async Task<IActionResult> UpdateProductRequests(ProductRequestDto requestDto)
        {
            await _adminService.UpdateProductRequestAsync(requestDto);
            return Ok();
        }

        /// <summary>
        /// Törli az igénylést
        /// </summary>
        /// <param name="id">Igénylés azonosítója</param>        
        [HttpDelete("delete-product-requests/{id}")]
        public async Task<IActionResult> DeleteProductRequests(int id)
        {
            await _adminService.DeleteProductRequestAsync(id);
            return Ok();
        }

        /// <summary>
        /// Lekéri a requesterekhez tartozó limit szabályokat.
        /// Ezek határozzák meg, hogy egy user hány kérést küldhet.
        /// </summary>        
        [HttpGet("requester-limitrules")]
        public async Task<IActionResult> GetRequesterLimitRules()
        {
            var limitRules = await _adminService.GetRequesterLimitRulesAsync();
            return Ok(limitRules);
        }

        /// <summary>
        /// Létrehozza az igénylés szabályát.
        /// </summary>
        /// <param name="limitRuleDto">A dto tartalmazza a szabály paramétereit.</param>        
        [HttpPost("create-requester-limitrule")]
        public async Task<IActionResult> CreateRequeserLimitRule(RequesterLimitRuleDto limitRuleDto)
        {
            await _adminService.CreateRequesterLimitRuleAsync(limitRuleDto);
            return Ok();
        }

        /// <summary>
        /// Módosítja a szabályt.
        /// </summary>
        /// <param name="limitRuleDto">>A dto tartalmazza a szabály paramétereit.</param>        
        [HttpPut("update-requester-limitrule")]
        public async Task<IActionResult> UpdateRequesterLimitRule(RequesterLimitRuleDto limitRuleDto)
        {
            await _adminService.UpdateRequesterLimitRuleAsync(limitRuleDto);
            return Ok();
        }

        /// <summary>
        /// Törli a szabályt.
        /// </summary>
        /// <param name="id">Szabély azonosítója.</param>        
        [HttpDelete("delete-requester-limitrule/{id}")]
        public async Task<IActionResult> DeleteRequesterLimitRule(int id)
        {
            await _adminService.DeleteRequesterLimitRuleAsync(id);
            return Ok();
        }
    }
}
