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

    public Task<User?> LoginAsync(string e, string p) => throw new NotImplementedException();
}