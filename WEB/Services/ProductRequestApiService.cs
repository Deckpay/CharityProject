using Application.DTOs;
using System.Net.Http.Headers;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace WEB.Services
{
    public class ProductRequestApiService
    {
        private readonly HttpClient _http;

        public ProductRequestApiService(HttpClient http) => _http = http;

        private static HttpRequestMessage CreateAuthRequest(HttpMethod method, string url, string token)
        {
            var request = new HttpRequestMessage(method, url);
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return request;
        }
        public async Task<IEnumerable<ProductRequestDto>> GetMyRequestsAsync(string token)
        {
            var request = CreateAuthRequest(HttpMethod.Get, "ProductRequest/my-requests", token);
            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new List<ProductRequestDto>();
            return await response.Content.ReadFromJsonAsync<IEnumerable<ProductRequestDto>>() ?? new List<ProductRequestDto>();
        }

        // Donor: az ő termékeihez beérkezett igénylések
        public async Task<IEnumerable<ProductRequestDto>> GetDonorRequestsAsync(string token)
        {
            var request = CreateAuthRequest(HttpMethod.Get, "ProductRequest/donor-requests", token);
            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new List<ProductRequestDto>();
            return await response.Content.ReadFromJsonAsync<IEnumerable<ProductRequestDto>>() ?? new List<ProductRequestDto>();
        }

        //  Igénylés törlése
        public async Task DeleteRequestAsync(int requestId, string token)
        {
            var request = CreateAuthRequest(HttpMethod.Delete, $"ProductRequest/request/{requestId}", token);
            await _http.SendAsync(request);
        }

        public async Task<int> ClaimProductAsync(int productId, string token)
        {
            var request = CreateAuthRequest(HttpMethod.Post, $"ProductRequest/claim/{productId}", token);
            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode) return 0;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("requestId", out var prop))
                return prop.GetInt32();

            return 0;
        }
    }
}
