using Application.DTOs;
using System.Net.Http.Headers;
using System.Text.Json;

namespace WEB.Services
{
    /// <summary>
    /// ProductRequest végpontok hívásáért felelős frontend API service.
    /// </summary>
    public class ProductRequestApiService
    {
        private readonly HttpClient _http;
        private readonly TokenStore _tokenStore;

        public ProductRequestApiService(HttpClient http, TokenStore tokenStore) 
        {
            _http = http;
            _tokenStore = tokenStore;
        }
        private void SetAuthHeader()
        {
            if (!string.IsNullOrWhiteSpace(_tokenStore.Token))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenStore.Token);
            }
        }

        private static HttpRequestMessage CreateAuthRequest(HttpMethod method, string url, string token)
        {

            var request = new HttpRequestMessage(method, url);
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return request;
        }

        public async Task<IEnumerable<ProductRequestDto>> GetMyRequestsAsync(string token)
        {
            SetAuthHeader();
            using var request = CreateAuthRequest(HttpMethod.Get, "ProductRequest/my-requests", token);
            using var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new List<ProductRequestDto>();
            return await response.Content.ReadFromJsonAsync<IEnumerable<ProductRequestDto>>() ?? new List<ProductRequestDto>();
        }

        // Sender: az ő termékeihez beérkezett igénylések
        public async Task<IEnumerable<ProductRequestDto>> GetSenderRequestsAsync(string token)
        {
            SetAuthHeader();
            using var request = CreateAuthRequest(HttpMethod.Get, "ProductRequest/Sender-requests", token);
            using var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new List<ProductRequestDto>();
            return await response.Content.ReadFromJsonAsync<IEnumerable<ProductRequestDto>>() ?? new List<ProductRequestDto>();
        }

        //  Igénylés törlése
        public async Task DeleteRequestAsync(int requestId, string token)
        {
            SetAuthHeader();
            using var request = CreateAuthRequest(HttpMethod.Delete, $"ProductRequest/request/{requestId}", token);
            await _http.SendAsync(request);
        }

        public async Task<ClaimResultDto> ClaimProductAsync(int productId, string token)
        {
            SetAuthHeader();
            using var request = CreateAuthRequest(HttpMethod.Post, $"ProductRequest/claim/{productId}", token);
            using var response = await _http.SendAsync(request);

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                // Próbáljuk deszerializálni a backend DTO-t
                var result = JsonSerializer.Deserialize<ClaimResultDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result != null)
                    return result;

                return new ClaimResultDto { Success = false, Message = "Hiba a szerverről." };
            }
            catch
            {
                // Ha nem sikerül deszerializálni, visszaadjuk a raw üzenetet
                return new ClaimResultDto { Success = false, Message = json };
            }
        }

        // Sender lezárja az igénylést. success=true → sikeres, false → sikertelen átadás.
        public async Task<bool> CompleteRequestAsync(int requestId, bool success, string token)
        {
            SetAuthHeader();
            var request = CreateAuthRequest(
                HttpMethod.Post,
                $"ProductRequest/complete/{requestId}?success={success.ToString().ToLower()}",
                token);
            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Megkeresi, hogy az adott termékre van-e már aktív (Pending) igénylés a bejelentkezett usertől.
        public async Task<int> GetActiveRequestIdForProductAsync(int productId, string token)
        {
            SetAuthHeader();
            using var request = CreateAuthRequest(
                HttpMethod.Get,
                $"ProductRequest/active-for-product/{productId}",
                token);
            using var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode) return 0;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("requestId", out var prop))
                return prop.GetInt32();

            return 0;
        }

        // Megkeresi, hogy az adott termékre van-e már aktív igénylés BÁRKI által.
        // true → foglalt (más user már igényelte), false → szabad
        public async Task<bool> IsProductClaimedAsync(int productId, string token)
        {
            SetAuthHeader();
            using var request = CreateAuthRequest(
                HttpMethod.Get,
                $"ProductRequest/is-claimed/{productId}",
                token);
            using var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode; // 200 = foglalt, 404 = szabad
        }
    }
}
