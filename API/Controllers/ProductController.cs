using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IProductService productService, IUnitOfWork unitOfWork)
        {
            _productService = productService;
            _unitOfWork = unitOfWork;
        }

        // 1. Összes termék lekérése
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetProductsAsync();
            return Ok(products); // Visszaküldjük a listát 200 OK-val
        }

        [Authorize] // Csak bejelentkezett felhasználó hívhatja
        [HttpGet("my-products")]
        public async Task<IActionResult> GetMyProducts()
        {
            // [Authorize] miatt a .NET automatikusan kiszedi a Usert-t a Tokenből
            // lekérjük a NameIdentifier claim-et
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var allProducts = await _unitOfWork.Products.GetAllAsync();
            var myProducts = allProducts.Where(p => p.DonorId.Equals(userId));

            return Ok(myProducts);
        }

        // 2. Új termék létrehozása
        [Authorize]
        [HttpPost]
        // MultipartFormData esetén a controller paramétereket érdemes explicit [FromForm]-mal jelölni
        public async Task<IActionResult> Create([FromForm]ProductDto dto, IFormFile imageFile)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                dto.ImagePath = "/images/products/" + fileName;
            }

            var succes = await _productService.CreateProductAsync(dto, int.Parse(userId));
            if (!succes) return BadRequest("Nem sikerült menteni");

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteProductAsync(id);
            return Ok();
        }
    }
}
