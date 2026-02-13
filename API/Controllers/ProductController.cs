using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // 1. Összes termék lekérése
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetProductsAsync();
            return Ok(products); // Visszaküldjük a listát 200 OK-val
        }

        // 2. Új termék létrehozása
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDto dto)
        {
            // A [FromBody] mondja meg az API-nak, hogy a JSON-t a kérés törzséből olvassa ki
            var success = await _productService.CreateProductAsync(dto);

            if (success) return Ok();
            return BadRequest("Nem sikerült a termék létrehozása.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteProductAsync(id);
            return Ok();
        }
    }
}
