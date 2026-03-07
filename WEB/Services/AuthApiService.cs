using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using static System.Net.WebRequestMethods;

namespace Web.Services
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

        public async Task<IEnumerable<County>> GetCountiesAsync()
        {
            return await _http.GetFromJsonAsync<IEnumerable<County>>("auth/counties") ?? new List<County>();
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

        public async Task<IEnumerable<ProductCategory>> GetProductCategoriesAsync()
        {
            return await _http.GetFromJsonAsync<IEnumerable<ProductCategory>>("auth/productcategories") ?? new List<ProductCategory>();
        }
    }
}
