namespace WEB.Services
{
    /// <summary>
    /// Kliensoldali JWT token ideiglenes tárolására szolgáló egyszerű state container.
    /// </summary>
    public class TokenStore
    {
        public string? Token { get; set; }
    }
}
