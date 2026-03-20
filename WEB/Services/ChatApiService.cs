using Application.DTOs;
using System.Net.Http.Headers;

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
    }
}
