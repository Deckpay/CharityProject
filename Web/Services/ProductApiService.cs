using Application.DTOs;
using Application.Interfaces;

namespace Web.Services
{
    public class ProductApiService(HttpClient http) : IProductService
    {
        // Itt csak egy sor az egész: "Postás, vidd ezt az API-nak!"
        public async Task<bool> CreateProductAsync(ProductDto productDto)
        {
            var response = await http.PostAsJsonAsync("Product", productDto);

            if (!response.IsSuccessStatusCode)
            {
                // Ez kiírja, hogy 400 (Rossz kérés) vagy 500 (Szerver hiba)
                Console.WriteLine($"Státusz kód: {response.StatusCode}");

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Részletes hiba: {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }

        // "Postás, hozz nekem listát az API-tól!"
        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            return await http.GetFromJsonAsync<IEnumerable<ProductDto>>("Product")
                   ?? new List<ProductDto>();
        }

        // "Postás, mondd meg az API-nak, hogy törölje a 5-öst!"
        public async Task DeleteProductAsync(int id)
        {
            await http.DeleteAsync($"Product/{id}");
        }
    }
}
