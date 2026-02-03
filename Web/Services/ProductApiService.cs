namespace Web.Services;

using Application.Interfaces;
using Application.DTOs;
using Domain.Entities;
using System.Net.Http.Json;

public class ProductApiService(HttpClient http) : IProductService
{
    // Itt csak egy sor az egész: "Postás, vidd ezt az API-nak!"
    public async Task<bool> CreateProductAsync(ProductDto productDto)
    {
        var response = await http.PostAsJsonAsync("api/products", productDto);
        return response.IsSuccessStatusCode;
    }

    // "Postás, hozz nekem listát az API-tól!"
    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await http.GetFromJsonAsync<IEnumerable<Product>>("api/products")
               ?? new List<Product>();
    }

    // "Postás, mondd meg az API-nak, hogy törölje a 5-öst!"
    public async Task DeleteProductAsync(int id)
    {
        await http.DeleteAsync($"api/products/{id}");
    }
}