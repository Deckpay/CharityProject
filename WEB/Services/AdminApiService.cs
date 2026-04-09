using Application.DTOs;

namespace WEB.Services
{
    /// <summary>
    /// Admin végpontok hívásáért felelős API service (frontend → backend).
    /// </summary>
    public class AdminApiService
    {
        private readonly HttpClient _httpClient;

        public AdminApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //user
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

        // product
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

        // request
        public async Task<IEnumerable<ProductRequestDto>> GetProductsRequestsAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<ProductRequestDto>>("admin/product-requests");
            return result ?? new List<ProductRequestDto>();
        }
        public async Task UpdateProductRequestAsync(ProductRequestDto requestDto)
        {
            await _httpClient.PutAsJsonAsync("admin/update-product-requests", requestDto);
        }
        public async Task DeleteProductRequestAsync(int id)
        {
            await _httpClient.PutAsync($"admin/delete-product-requests/{id}", null);
        }

        // limit rule
        public async Task<IEnumerable<RequesterLimitRuleDto>> GetRequesterLimitRulesAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<RequesterLimitRuleDto>>("admin/requester-limitrules");
            return result ?? new List<RequesterLimitRuleDto>();
        }
        public async Task UpdateRequesterLimitRuleAsync(RequesterLimitRuleDto limitRuleDto)
        {
            await _httpClient.PutAsJsonAsync("admin/update-requester-limitrule", limitRuleDto);
        }
        public async Task CreateRequesterLimitRuleAsync(RequesterLimitRuleDto limitRuleDto)
        {
            await _httpClient.PostAsJsonAsync("admin/create-requester-limitrule", limitRuleDto);
        }
        public async Task DeleteRequesterLimitRuleAsync(int id)
        {
            await _httpClient.PutAsync($"admin/delete-requester-limitrule/{id}", null);
        }
    }
}
