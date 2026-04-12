using Application.DTOs;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;

namespace WEB.Services
{
    /// <summary>
    /// Product végpontok hívásáért felelős frontend API service.
    /// </summary>
    public class ProductApiService
    {
        private readonly HttpClient _http;

        public ProductApiService(HttpClient http)
        {
            _http = http;
        }

        private static HttpRequestMessage CreateAuthRequest(HttpMethod method, string url, string token)
        {
            var request = new HttpRequestMessage(method, url);

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return request;
        }

        public async Task<bool> CreateProductAsync(ProductDto productDto, IBrowserFile imageFile, string token)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(productDto.ProductName), "ProductName");
            content.Add(new StringContent(productDto.ProductDescription ?? string.Empty), "ProductDescription");
            content.Add(new StringContent(productDto.ProductCategoryId.ToString()), "ProductCategoryId");
            content.Add(new StringContent(productDto.CountyId.ToString()), "CountyId");

            using var stream = imageFile.OpenReadStream(10 * 1024 * 1024);
            using var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
            content.Add(fileContent, "ImageFile", imageFile.Name);

            using var request = CreateAuthRequest(HttpMethod.Post, "Product", token);
            request.Content = content;

            using var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync(string token)
        {
            using var request = CreateAuthRequest(HttpMethod.Get, "Product", token);
            using var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return new List<ProductDto>();

            return await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>()
                   ?? new List<ProductDto>();
        }

        public async Task<IEnumerable<ProductDto>> GetMyProductsAsync(string token)
        {
            using var request = CreateAuthRequest(HttpMethod.Get, "Product/my-products", token);
            using var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return new List<ProductDto>();

            return await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>()
                   ?? new List<ProductDto>();
        }

        public async Task<bool> UpdateProductAsync(ProductDto productDto, string token)
        {
            using var request = CreateAuthRequest(HttpMethod.Put, "Product/update-product", token);
            request.Content = JsonContent.Create(productDto);

            using var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProductAsync(int id, string token)
        {
            using var request = CreateAuthRequest(HttpMethod.Delete, $"Product/{id}", token);
            using var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}