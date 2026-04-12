using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Lekéri az összes terméket.
        /// </summary>
        /// <returns>200 OK a termékek listájával.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _productService.GetProductsAsync());

        /// <summary>
        /// Lekéri a bejelentkezett felhasználó saját termékeit.
        /// </summary>
        /// <returns>200 OK a felhasználó termékeivel.</returns>
        [HttpGet("my-products")]
        public async Task<IActionResult> GetMyProducts()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Ok(await _productService.GetProductsBySenderAsync(userId));
        }

        /// <summary>
        /// Új termék létrehozása.
        /// </summary>
        /// <param name="dto">A termék adatai.</param>
        /// <param name="imageFile">A termékhez tartozó kép.</param>
        /// <returns>200 OK, ha sikeres; 400 BadRequest hiba esetén.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductDto dto, IFormFile imageFile)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var success = await _productService.CreateProductAsync(dto, userId, imageFile);
            return success ? Ok() : BadRequest("Hiba a mentés során.");
        }

        /// <summary>
        /// Termék adatainak frissítése.
        /// </summary>
        /// <param name="productDto">A módosítandó termék adatai.</param>
        /// <returns>200 OK, ha a frissítés sikeres.</returns>
        [HttpPut("update-product")]
        public async Task<IActionResult> UpdateProducts(ProductDto productDto)
        {
            await _productService.UpdateProductAsync(productDto);
            return Ok();
        }

        /// <summary>
        /// Termék törlése (soft delete).
        /// </summary>
        /// <param name="id">A törlendő termék azonosítója.</param>
        /// <returns>200 OK, ha a törlés sikeres.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteProductAsync(id);
            return Ok();
        }
    }
}
