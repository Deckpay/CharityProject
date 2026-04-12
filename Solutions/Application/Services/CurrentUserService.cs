using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Services
{
    /// <summary>
    /// A bejelentkezett felhasználó adatait biztosító szolgáltatás.
    /// A HttpContext-ből olvassa ki a felhasználó azonosítóját.
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public int UserId
        {
            get
            {
                var id = _contextAccessor.HttpContext?.User?
                    .Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                return string.IsNullOrEmpty(id) ? 0 : int.Parse(id);
            }
        }
    }
}