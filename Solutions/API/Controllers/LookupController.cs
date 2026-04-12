using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LookupController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public LookupController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Lekéri az összes termékkategóriát.
        /// </summary>
        /// <returns>200 OK a termékkategóriák listájával.</returns>
        [HttpGet("product-categories")]
        public async Task<IActionResult> GetProductCategories()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Lekéri az összes megyét.
        /// </summary>
        /// <returns>200 OK a megyék listájával.</returns>
        [HttpGet("counties")]
        public async Task<IActionResult> GetCounties()
        {
            var counties = await _unitOfWork.Counties.GetAllAsync();
            return Ok(counties);
        }

        /// <summary>
        /// Lekéri a felhasználók alapadatait.
        /// </summary>
        /// <returns>200 OK a felhasználók listájával.</returns>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _unitOfWork.Users.GetAllAsync();

            var userDots = users.Select(u => new UserDto
            {
                UserId = u.UserId,
                UserName = u.UserName

            });

            return Ok(userDots);
        }

        /// <summary>
        /// Lekéri az összes terméket.
        /// </summary>
        /// <returns>200 OK a termékek listájával.</returns>
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return Ok(products);
        }
    }
}
