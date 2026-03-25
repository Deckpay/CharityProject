using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _adminService.GetUsersAsync();
            return Ok(users);
        }

        [HttpPut("ban-user/{id}")]
        public async Task<IActionResult> BanUser(int id)
        {
            await _adminService.BanUserAsync(id);
            return Ok();
        }

        [HttpPut("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _adminService.DeleteUserAsync(id);
            return Ok();
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser(UserDto userDto)
        {
            await _adminService.UpdateUserAsync(userDto);
            return Ok();
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _adminService.GetProductsAsync();
            return Ok(products);
        }

        [HttpPut("update-product")]
        public async Task<IActionResult> UpdateProducts(ProductDto productDto)
        {
            await _adminService.UpdateProductAsync(productDto);
            return Ok();
        }

        [HttpPut("delete-product/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _adminService.DeleteProductAsync(id);
            return Ok();
        }

        [HttpGet("product-requests")]
        public async Task<IActionResult> GetProductRequests()
        {
            var requests = await _adminService.GetProductRequestsAsync();
            return Ok(requests);
        }

        [HttpPut("update-product-requests")]
        public async Task<IActionResult> UpdateProductRequests(ProductRequestDto requestDto)
        {
            await _adminService.UpdateProductRequestAsync(requestDto);
            return Ok();
        }

        [HttpPut("delete-product-requests/{id}")]
        public async Task<IActionResult> DeleteProductRequests(int id)
        {
            await _adminService.DeleteProductRequestAsync(id);
            return Ok();
        }

        [HttpGet("requester-limitrules")]
        public async Task<IActionResult> GetRequesterLimitRules()
        {
            var limitRules = await _adminService.GetRequesterLimitRules();
            return Ok(limitRules);
        }

        [HttpPost("create-requester-limitrule")]
        public async Task<IActionResult> CreateRequesertLimitRule(RequesterLimitRuleDto limitRuleDto)
        {
            await _adminService.CreateRequesterLimitRule(limitRuleDto);
            return Ok();
        }

        [HttpPut("update-requester-limitrule")]
        public async Task<IActionResult> UpdateRequesertLimitRule(RequesterLimitRuleDto limitRuleDto)
        {
            await _adminService.UpdateRequesterLimitRule(limitRuleDto);
            return Ok();
        }

        [HttpPut("delete-requester-limitrule/{id}")]
        public async Task<IActionResult> DeleteRequesterLimitRule(int id)
        {
            await _adminService.DeleteRequesterLimitRule(id);
            return Ok();
        }
    }
}
