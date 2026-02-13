using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Web.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        // Itt hozzuk létre a belső változót, hogy az egész osztály lássa
        private readonly LocalStorageService _storage;

        // A konstruktorban kapjuk meg a storage-ot és elmentjük a fenti változóba
        public CustomAuthStateProvider(LocalStorageService storage)
        {
            _storage = storage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // Mostantól az _storage változót használjuk, ami biztosan létezik
                var userName = await _storage.GetItemAsync<string>("currentUser");

                if (string.IsNullOrEmpty(userName))
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                var claims = new List<Claim> { new Claim(ClaimTypes.Name, userName) };
                var identity = new ClaimsIdentity(claims, "CustomAuth");
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch
            {
                // Ha hiba van (pl. Prerendering), kijelentkezett állapotot adunk
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public void NotifyUserLogin(string userName)
        {
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userName) }, "CustomAuth");
            var userPrincipal = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(userPrincipal)));
        }

        public void NotifyUserLogout()
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
        }
    }
}
