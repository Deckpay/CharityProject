using Application.DTOs;

namespace WEB.Services
{
    public class AuthApiService
    {
        private readonly HttpClient _http;
        public AuthApiService(HttpClient http)
        {
            _http = http;
        }
        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            // Az appsettings-ben lévő BaseUrl-hez (api/) hozzáfűzzük az auth/register-t
            var response = await _http.PostAsJsonAsync("auth/register", dto);
            return response.IsSuccessStatusCode;
        }

        // LOGIN → JWT TOKEN-t ad vissza
        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var response = await _http.PostAsJsonAsync("auth/login", dto);

            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

            return result?.Token;
        }
    }
}
