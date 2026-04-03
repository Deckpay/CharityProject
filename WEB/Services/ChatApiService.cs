using Application.DTOs;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace WEB.Services
{
    public class ChatApiService
    {
        private readonly HttpClient _http;

        public ChatApiService(HttpClient http)
        {
            _http = http;
        }

        private static HttpRequestMessage CreateAuthRequest(HttpMethod method, string url, string token)
        {
            var request = new HttpRequestMessage(method, url);
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return request;
        }

        public async Task<List<ChatMessageResponseDto>> GetHistoryAsync(int requestId, string token)
        {
            var request = CreateAuthRequest(HttpMethod.Get, $"Chat/history/{requestId}", token);
            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new List<ChatMessageResponseDto>();
            return await response.Content.ReadFromJsonAsync<List<ChatMessageResponseDto>>() ?? new();
        }

        public async Task<bool> SendMessageAsync(int requestId, string content, string token)
        {
            var dto = new ChatMessageRequestDto
            {
                RequestId = requestId,
                Content = content
            };
            var request = CreateAuthRequest(HttpMethod.Post, "Chat/send", token);
            request.Content = JsonContent.Create(dto);
            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// asasasasa
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ProductRequestDto?> GetRequestDetailsAsync(int requestId, string token)
        {
            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            // Az útvonalat ellenőrizd az API kontrolleredben (pl. api/ProductRequest/{id})
            var response = await _http.GetAsync($"ProductRequest/{requestId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProductRequestDto>();
            }
            return null;
        }

        /// <summary>
        /// sasasasa
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ProductDto?> GetProductByIdAsync(int productId, string token)
        {
            var request = CreateAuthRequest(HttpMethod.Get, $"Product/{productId}", token);
            var response = await _http.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProductDto>();
            }
            return null;
        }

        public async Task<ChatInfoDto?> GetChatInfoAsync(int requestId, string token)
        {
            var request = CreateAuthRequest(HttpMethod.Get, $"Chat/info/{requestId}", token);
            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<ChatInfoDto>();
        }


        public async Task<int> GetUnreadCountAsync(string token)
        {
            try
            {
                var request = CreateAuthRequest(HttpMethod.Get, "Chat/unread-count", token);
                var response = await _http.SendAsync(request);
                if (!response.IsSuccessStatusCode) return 0;
                var raw = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"unread-count raw response: '{raw}'");
                return int.Parse(raw);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetUnreadCountAsync hiba: {ex.Message}");
                return 0;
            }
        }

        public async Task MarkAsReadAllAsync(int requestId, string token)
        {
            var request = CreateAuthRequest(HttpMethod.Post, $"Chat/mark-read/{requestId}", token);
            await _http.SendAsync(request);
        }
    }
}
