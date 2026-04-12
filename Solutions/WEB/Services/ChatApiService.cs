using Application.DTOs;
using System.Net.Http.Headers;

namespace WEB.Services
{
    /// <summary>
    /// Chat végpontok hívásáért felelős frontend API service.
    /// </summary>
    public class ChatApiService
    {
        private readonly HttpClient _http;

        public ChatApiService(HttpClient http)
        {
            _http = http;
        }

        /// <summary>
        /// Auth headerrel ellátott HTTP kérés létrehozása.
        /// </summary>
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
                return int.Parse(raw);
            }
            catch (Exception ex)
            {
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
