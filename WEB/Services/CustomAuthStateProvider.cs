using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WEB.Services
{
    /// <summary>
    /// A kliensoldali hitelesítési állapot kezeléséért felelős provider.
    /// A JWT token alapján építi fel az aktuális felhasználót.
    /// </summary>
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly LocalStorageService _localStorage;
        private readonly TokenStore _tokenStore;

        public CustomAuthStateProvider(LocalStorageService localStorage, TokenStore tokenStore)
        {
            _localStorage = localStorage;
            _tokenStore = tokenStore;
        }

        /// <summary>
        /// Visszaadja az aktuális hitelesítési állapotot a tárolt JWT token alapján.
        /// </summary>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");

                if (string.IsNullOrWhiteSpace(token))
                    return new AuthenticationState(CreateAnonymous());

                if (IsTokenExpired(token))
                {
                    await _localStorage.RemoveItemAsync("authToken");
                    _tokenStore.Token = null;
                    return new AuthenticationState(CreateAnonymous());
                }

                _tokenStore.Token = token;

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                var identity = new ClaimsIdentity(
                    jwt.Claims,
                    "jwt",
                    ClaimTypes.Name,
                    ClaimTypes.Role
                );

                var user = new ClaimsPrincipal(identity);

                return new AuthenticationState(user);
            }
            catch
            {
                return new AuthenticationState(CreateAnonymous());
            }
        }

        /// <summary>
        /// Bejelentkezés után frissíti az auth állapotot az új token alapján.
        /// </summary>
        public void NotifyUserAuthentication(string token)
        {
            if (string.IsNullOrWhiteSpace(token) || IsTokenExpired(token))
            {
                NotifyUserLogout();
                return;
            }

            // Frissítsük a TokenStore-ot is, hogy a TokenHandler és az HttpClient-ek
            // azonnal lássák az új tokent.
            _tokenStore.Token = token;

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var identity = new ClaimsIdentity(
                    jwt.Claims,
                    "jwt",
                    ClaimTypes.Name,
                    ClaimTypes.Role
                );

            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(user)));
        }

        public void NotifyUserLogout()
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(anonymous)));
        }

        /// <summary>
        /// Kijelentkezteti a felhasználót, törli a tokent és anonim állapotot állít be.
        /// </summary>
        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _tokenStore.Token = null;

            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(anonymousUser))
            );
        }

        public async Task<int> GetCurrentUserIdAsync()
        {
            var state = await GetAuthenticationStateAsync();
            var user = state.User;

            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(idClaim, out var id) ? id : 0;
        }

        /// <summary>
        /// Ellenőrzi, hogy a token lejárt-e.
        /// </summary>
        private bool IsTokenExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
                return true;

            var jwt = handler.ReadJwtToken(token);

            return jwt.ValidTo <= DateTime.UtcNow;
        }

        private ClaimsPrincipal CreateAnonymous()
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }
        public async Task<bool> ValidateTokenAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
                return false;

            if (IsTokenExpired(token))
            {
                await LogoutAsync();
                return false;
            }

            return true;
        }
    }
}
