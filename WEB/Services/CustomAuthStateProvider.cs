using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WEB.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly LocalStorageService _localStorage;
        private readonly TokenStore _tokenStore;

        public CustomAuthStateProvider(LocalStorageService localStorage, TokenStore tokenStore)
        {
            _localStorage = localStorage;
            _tokenStore = tokenStore;
        }

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

                // KRITIKUS: A TokenStore-t is frissíteni kell, hogy a többi Service lássa!
                _tokenStore.Token = token;

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                //var identity = new ClaimsIdentity(jwt.Claims, "jwt");
                //var user = new ClaimsPrincipal(identity);

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
                // prerender alatt JS nem működik
                return new AuthenticationState(CreateAnonymous());
            }
        }

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

            //var identity = new ClaimsIdentity(jwt.Claims, "jwt");

            //var user = new ClaimsPrincipal(identity);

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

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _tokenStore.Token = null; // TokenStore törlése.

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
