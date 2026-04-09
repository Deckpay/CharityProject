using System.Net.Http.Headers;

namespace WEB.Services
{
    /// <summary>
    /// Automatikusan hozzáadja a JWT tokent minden kimenő HTTP kéréshez.
    /// </summary>
    public class TokenHandler : DelegatingHandler
    {
        private readonly TokenStore _tokenStore;

        public TokenHandler(TokenStore tokenStore)
        {
            _tokenStore = tokenStore;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = _tokenStore.Token;

            if (!string.IsNullOrWhiteSpace(token))
            {
                // A szóköz automatikusan bekerül a séma és a token közé, 
                // ne írj szóközt a "Bearer" szó után!
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
