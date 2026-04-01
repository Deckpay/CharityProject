using Application.DTOs;
using Domain.Entities;

namespace WEB.Services
{
    public class LimitApiService
    {
        private readonly HttpClient _httpClient;

        public LimitApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CanRequestAsync(int userId, int categoryId)
        {
            var response = await _httpClient.GetFromJsonAsync<LimitResponseDto>(
                $"api/limit/can-request?userId={userId}&categoryId={categoryId}");

            return response?.CanRequest ?? false;
        }

        public async Task<bool> UseLimitAsync(int userId, int categoryId)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/limit/use",
                new { userId, categoryId });

            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<UseLimitResponseDto>();
            return result?.Success ?? false;
        }
    }
}
