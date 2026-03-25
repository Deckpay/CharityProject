using Application.DTOs;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Text.Json;

namespace WEB.Services
{
    public class ProductApiService
    {
        private readonly HttpClient _http;

        public ProductApiService(HttpClient http) => _http = http;

        private static HttpRequestMessage CreateAuthRequest(HttpMethod method, string url, string token)
        {
            var request = new HttpRequestMessage(method, url);
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return request;
        }

        public async Task<bool> CreateProductAsync(ProductDto productDto, IBrowserFile imageFile, string token)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(productDto.ProductName), "ProductName");
            content.Add(new StringContent(productDto.ProductDescription ?? ""), "ProductDescription");
            content.Add(new StringContent(productDto.ProductCategoryId.ToString()), "ProductCategoryId");
            content.Add(new StringContent(productDto.CountyId.ToString()), "CountyId");

            var stream = imageFile.OpenReadStream(10 * 1024 * 1024);
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
            content.Add(fileContent, "ImageFile", imageFile.Name);

            var request = new HttpRequestMessage(HttpMethod.Post, "Product") { Content = content };
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            return await _http.GetFromJsonAsync<IEnumerable<ProductDto>>("Product")
                   ?? new List<ProductDto>();
        }

        public async Task<IEnumerable<ProductDto>> GetMyProductsAsync(string token)
        {
            var request = CreateAuthRequest(HttpMethod.Get, "Product/my-products", token);
            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new List<ProductDto>();
            return await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>() ?? new List<ProductDto>();
        }

        public async Task DeleteProductAsync(int id, string token)
        {
            var request = CreateAuthRequest(HttpMethod.Delete, $"Product/{id}", token);
            await _http.SendAsync(request);
        }

       
    }
}