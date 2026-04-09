using Domain.Entities;

namespace WEB.Services
{
    /// <summary>
    /// Lookup adatok lekérdezéséért és kliensoldali gyorsítótárazásáért felelős API service.
    /// </summary>
    public class LookupApiService
    {
        private readonly HttpClient _httpClient;

        public LookupApiService(HttpClient httpClient)
        {
            _httpClient = httpClient; 
        }

        private List<ProductCategory>? _productCategories;
        private List<County>? _counties;
        private List<User>? _users;
        private List<Product>? _products;

        public async Task<IEnumerable<ProductCategory>> GetProductCategoriesAsync()
        {
            if (_productCategories == null)
            {
                _productCategories = await _httpClient.GetFromJsonAsync<List<ProductCategory>>("lookup/product-categories") ?? new();
            }

            return _productCategories;
        }

        public async Task<IEnumerable<County>> GetCountiesAsync()
        {
            if (_counties == null)
            {
                _counties = await _httpClient.GetFromJsonAsync<List<County>>("lookup/counties") ?? new();
            }

            return _counties;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            if (_users == null)
            {
                _users = await _httpClient.GetFromJsonAsync<List<User>>("lookup/users") ?? new();
            }

            return _users;
        }
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            if (_products == null)
            {
                _products = await _httpClient.GetFromJsonAsync<List<Product>>("lookup/products") ?? new();
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
