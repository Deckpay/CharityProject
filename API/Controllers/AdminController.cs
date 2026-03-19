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
    }
}
