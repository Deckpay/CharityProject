using Application.DTOs;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;

namespace WEB.Services
{
    public class ProductApiService
    {
        private readonly HttpClient _http;
        public ProductApiService(HttpClient http) { _http = http; }

        // Itt csak egy sor az egész: "Postás, vidd ezt az API-nak!"
        public async Task<bool> CreateProductAsync(ProductDto productDto, IBrowserFile imageFile)
        {
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(productDto.ProductName), "ProductName");
            content.Add(new StringContent(productDto.ProductDescription ?? ""), "ProductDescription");
            content.Add(new StringContent(productDto.ProductCategoryId.ToString()), "ProductCategoryId");
            content.Add(new StringContent(productDto.CountyId.ToString()), "CountyId");

            var stream = imageFile.OpenReadStream(10 * 1024 * 1024);

            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType =
                new MediaTypeHeaderValue(imageFile.ContentType);

            content.Add(fileContent, "ImageFile", imageFile.Name);

            var response = await _http.PostAsync("Product", content);

            return response.IsSuccessStatusCode;
        }

        // "Postás, hozz nekem listát az API-tól!"
        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            return await _http.GetFromJsonAsync<IEnumerable<ProductDto>>("Product")
                   ?? new List<ProductDto>();
        }

        public async Task<IEnumerable<ProductDto>> GetMyProductsAsync()
        {
            return await _http.GetFromJsonAsync<IEnumerable<ProductDto>>("Product/my-products") ?? new List<ProductDto>();
        }

        // "Postás, mondd meg az API-nak, hogy törölje a 5-öst!"
        public async Task DeleteProductAsync(int id)
        {
            await _http.DeleteAsync($"Product/{id}");
        }
    }
}
