using Application.DTOs;

namespace WEB.Services
{
    /// <summary>
    /// Admin végpontok hívásáért felelős API service (frontend → backend).
    /// </summary>
    public class AdminApiService
    {
        private readonly HttpClient _httpClient;
        private readonly TokenStore _tokenStore;

        public AdminApiService(HttpClient httpClient, TokenStore tokenStore)
        {
            _httpClient = httpClient;
            _tokenStore = tokenStore;
        }

        // Segédmetódus: minden kérés előtt beállítja a tokent
        private void SetAuthHeader()
        {
            if (!string.IsNullOrWhiteSpace(_tokenStore.Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenStore.Token);
            }
        }

        //user
        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            SetAuthHeader();
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<UserDto>>("admin/users");
            return result ?? new List<UserDto>();
        }

        public async Task BanUserAsync(int id)
        {
            SetAuthHeader();
            await _httpClient.PutAsync($"admin/ban-user/{id}", null);
        }

        public async Task DeleteUserAsync(int id)
        {
            SetAuthHeader();
            await _httpClient.DeleteAsync($"admin/delete-user/{id}");
        }

        public async Task UpdateUserAsync(UserDto userDto)
        {
            SetAuthHeader();
            await _httpClient.PutAsJsonAsync("admin/update-user", userDto);
        }

        // product
        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            SetAuthHeader();
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<ProductDto>>("admin/products");
            return result ?? new List<ProductDto>();
        }
        public async Task UpdateProductAsync(ProductDto productDto)
        {
            SetAuthHeader();
            await _httpClient.PutAsJsonAsync("admin/update-product", productDto);
        }
        public async Task DeleteProductAsync(int id)
        {
            SetAuthHeader();
            await _httpClient.DeleteAsync($"admin/delete-product/{id}");
        }

        // request
        public async Task<IEnumerable<ProductRequestDto>> GetProductsRequestsAsync()
        {
            SetAuthHeader();
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<ProductRequestDto>>("admin/product-requests");
            return result ?? new List<ProductRequestDto>();
        }
        public async Task UpdateProductRequestAsync(ProductRequestDto requestDto)
        {
            SetAuthHeader();
            await _httpClient.PutAsJsonAsync("admin/update-product-requests", requestDto);
        }
        public async Task DeleteProductRequestAsync(int id)
        {
            SetAuthHeader();
            await _httpClient.DeleteAsync($"admin/delete-product-requests/{id}");
        }

        // limit rule
        public async Task<IEnumerable<RequesterLimitRuleDto>> GetRequesterLimitRulesAsync()
        {
            SetAuthHeader();
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<RequesterLimitRuleDto>>("admin/requester-limitrules");
            return result ?? new List<RequesterLimitRuleDto>();
        }
        public async Task UpdateRequesterLimitRuleAsync(RequesterLimitRuleDto limitRuleDto)
        {
            SetAuthHeader();
            await _httpClient.PutAsJsonAsync("admin/update-requester-limitrule", limitRuleDto);
        }
        public async Task CreateRequesterLimitRuleAsync(RequesterLimitRuleDto limitRuleDto)
        {
            SetAuthHeader();
            await _httpClient.PostAsJsonAsync("admin/create-requester-limitrule", limitRuleDto);
        }
        public async Task DeleteRequesterLimitRuleAsync(int id)
        {
            SetAuthHeader();
            await _httpClient.DeleteAsync($"admin/delete-requester-limitrule/{id}");
        }
    }
}
