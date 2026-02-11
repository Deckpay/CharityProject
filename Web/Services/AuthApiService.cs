namespace Web.Services;

using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using System.Net.Http.Json;

public class AuthApiService(HttpClient http) : IAuthService
{
    public async Task<bool> RegisterAsync(RegisterDto dto)
    {
        // Az appsettings-ben lévő BaseUrl-hez (api/) hozzáfűzzük az auth/register-t
        var response = await http.PostAsJsonAsync("auth/register", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<County>> GetCountiesAsync()
    {
        return await http.GetFromJsonAsync<IEnumerable<County>>("auth/counties") ?? new List<County>();
    }

    public async Task<User?> LoginAsync(string emailOrUserName, string password)
    {
        var loginDto = new LoginDto { EmailOrUserName = emailOrUserName, Password = password };

        // Ez kiírja a Visual Studio 'Output' ablakába a TELJES címet:
        Console.WriteLine($"DEBUG: Hívott URL: {http.BaseAddress}auth/login");

        var response = await http.PostAsJsonAsync("auth/login", loginDto);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<User>();
        }
        return null;
    }

    public async Task<IEnumerable<ProductCategory>> GetProductCategoriesAsync()
    {
        return await http.GetFromJsonAsync<IEnumerable<ProductCategory>>("auth/productcategories") ?? new List<ProductCategory>();
    }

    //public async Task<User?> LoginAsync(string emailOrUserName, string password)
    //{
    //    var loginDto = new LoginDto { EmailOrUserName = emailOrUserName, Password = password };
    //    var response = await http.PostAsJsonAsync("api/auth/login", loginDto);

    //    if (response.IsSuccessStatusCode)
    //    {
    //        return await response.Content.ReadFromJsonAsync<User>();
    //    }
    //    return null;
    //}
}