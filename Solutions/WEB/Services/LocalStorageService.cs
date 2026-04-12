using Microsoft.JSInterop;
using System.Text.Json;

namespace WEB.Services
{
    /// <summary>
    /// LocalStorage elérés JSInterop segítségével.
    /// JSON formátumban tárol és olvas adatokat.
    /// </summary>
    public class LocalStorageService
    {
        private readonly IJSRuntime _js;
        public LocalStorageService(IJSRuntime js) { _js = js; }

        /// <summary>
        /// Érték mentése a localStorage-ba JSON formátumban.
        /// </summary>
        public async Task SetItemAsync<T>(string key, T value)
        {
            await _js.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));
        }

        /// <summary>
        /// Érték lekérése és visszaalakítása a megadott típusra.
        /// </summary>
        public async Task<T?> GetItemAsync<T>(string key)
        {
            // Lekérjük a szöveget, és ha létezik, visszalakítjuk C# objektummá.
            var json = await _js.InvokeAsync<string>("localStorage.getItem", key);
            return json == null ? default : JsonSerializer.Deserialize<T>(json);
        }

        /// <summary>
        /// Érték mentése a localStorage-ba JSON formátumban.
        /// </summary>
        public async Task RemoveItemAsync(string key)
        {
            // Törlés kijelentkezéskor.
            await _js.InvokeVoidAsync("localStorage.removeItem", key);
        }
    }
}
