using Microsoft.JSInterop;
using System.Text.Json;

namespace Web.Services
{
    public class LocalStorageService(IJSRuntime js)
    {
        // A JSInterop (IJSRuntime) segítségével "átkiabálunk" a böngészőnek.

        public async Task SetItemAsync<T>(string key, T value)
        {
            // A C# objektumot (pl. User) JSON szöveggé alakítjuk, mert a böngésző csak szöveget tud tárolni.
            await js.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));
        }

        public async Task<T?> GetItemAsync<T>(string key)
        {
            // Lekérjük a szöveget, és ha létezik, visszalakítjuk C# objektummá.
            var json = await js.InvokeAsync<string>("localStorage.getItem", key);
            return json == null ? default : JsonSerializer.Deserialize<T>(json);
        }

        public async Task RemoveItemAsync(string key)
        {
            // Törlés kijelentkezéskor.
            await js.InvokeVoidAsync("localStorage.removeItem", key);
        }
    }
}
