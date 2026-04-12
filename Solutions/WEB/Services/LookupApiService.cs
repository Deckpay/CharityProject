using Domain.Entities;
using System.Net.Http.Headers;

namespace WEB.Services
{
    /// <summary>
    /// Lookup adatok lekérdezéséért és kliensoldali gyorsítótárazásáért felelős API service.
    /// </summary>
    public class LookupApiService
    {
        private readonly HttpClient _httpClient;
        private readonly TokenStore _tokenStore;

        public LookupApiService(HttpClient httpClient, TokenStore tokenStore)
        {
            _httpClient = httpClient;
            _tokenStore = tokenStore;
        }

        private List<ProductCategory>? _productCategories;
        private List<County>? _counties;
        private List<User>? _users;
        private List<Product>? _products;

        /// <summary>
        /// Létrehoz egy HttpRequestMessage-t a tárolt JWT tokennel,
        /// ugyanúgy ahogy a ProductApiService és ChatApiService teszi.
        /// </summary>
        private HttpRequestMessage CreateAuthRequest(HttpMethod method, string url)
        {
            var request = new HttpRequestMessage(method, url);
            var token = _tokenStore.Token;
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return request;
        }

        public async Task<IEnumerable<ProductCategory>> GetProductCategoriesAsync()
        {
            if (_productCategories == null)
            {
                using var request = CreateAuthRequest(HttpMethod.Get, "lookup/product-categories");
                using var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    return new List<ProductCategory>();
                _productCategories = await response.Content.ReadFromJsonAsync<List<ProductCategory>>() ?? new();
            }
            return _productCategories;
        }

        public async Task<IEnumerable<County>> GetCountiesAsync()
        {
            if (_counties == null)
            {
                using var request = CreateAuthRequest(HttpMethod.Get, "lookup/counties");
                using var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    return new List<County>();
                _counties = await response.Content.ReadFromJsonAsync<List<County>>() ?? new();
            }
            return _counties;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            if (_users == null)
            {
                using var request = CreateAuthRequest(HttpMethod.Get, "lookup/users");
                using var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    return new List<User>();
                _users = await response.Content.ReadFromJsonAsync<List<User>>() ?? new();
            }
            return _users;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            if (_products == null)
            {
                using var request = CreateAuthRequest(HttpMethod.Get, "lookup/products");
                using var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    return new List<Product>();
                _products = await response.Content.ReadFromJsonAsync<List<Product>>() ?? new();
            }
            return _products;
        }

        public string GetProductCategoryName(int id) =>
            _productCategories?.FirstOrDefault(c => c.ProductCategoryId == id)?.ProductCategoryName ?? "Ismeretlen";

        public string GetCountyName(int id) =>
            _counties?.FirstOrDefault(c => c.CountyId == id)?.CountyName ?? "Ismeretlen";

        public string GetUserName(int id) =>
            _users?.FirstOrDefault(u => u.UserId == id)?.UserName ?? "Ismeretlen";

        public string GetProductName(int id) =>
            _products?.FirstOrDefault(u => u.ProductId == id)?.ProductName ?? "Ismeretlen";
    }
}
