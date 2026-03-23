using Application.DTOs;

namespace WEB.Services
{
    public class AdminApiService
    {
        private readonly HttpClient _httpClient;

        public AdminApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<UserDto>>("admin/users");
            return result ?? new List<UserDto>();
        }

        public async Task BanUserAsync(int id)
        {
            await _httpClient.PutAsync($"admin/ban-user/{id}", null);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _httpClient.PutAsync($"admin/delete-user/{id}", null);
        }

        public async Task UpdateUserAsync(UserDto userDto)
        {
            await _httpClient.PutAsJsonAsync("admin/update-user", userDto);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<ProductDto>>("admin/products");
            return result ?? new List<ProductDto>();
        }
        public async Task UpdateProductAsync(ProductDto productDto)
        {
            await _httpClient.PutAsJsonAsync("admin/update-product", productDto);
        }
        public async Task DeleteProductAsync(int id)
        {
            await _httpClient.PutAsync($"admin/delete-product/{id}", null);
        }
    }
}
