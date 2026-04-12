using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthController(IAuthService authService,IJwtTokenGenerator jwtTokenGenerator)
        {
            _authService = authService;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        /// <summary>
        /// Új felhasználó regisztrációja.
        /// </summary>
        /// <param name="dto">A regisztrációhoz szükséges adatok.</param>
        /// <returns>200 OK, ha sikeres; 400 BadRequest, ha sikertelen.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var success = await _authService.RegisterAsync(dto);

            if (success) return Ok();
            return BadRequest("A regisztráció sikertelen");
        }

        /// <summary>
        /// Felhasználó bejelentkezése.
        /// </summary>
        /// <param name="dto">Email vagy felhasználónév és jelszó.</param>
        /// <returns>JWT token sikeres bejelentkezés esetén.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginDto dto)
        {
            var user = await _authService.LoginAsync(dto.EmailOrUserName, dto.Password);

            if (user == null)
                return Unauthorized();

            Console.WriteLine($"LOGIN TOKEN: {user.Token}");

            return Ok(user);
        }

        /// <summary>
        /// A bejelentkezett felhasználó saját fiókjának törlése.
        /// </summary>
        /// <returns>200 OK, ha sikeres; 400 vagy 401 hiba esetén.</returns>
        [Authorize]
        [HttpDelete("delete-my-account")]
        public async Task<IActionResult> DeleteMyAccount()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Érvénytelen token");

            var success = await _authService.DeleteMyAccountAsync(userId);

            if (!success)
                return BadRequest("A fiók törlése sikertelen.");

            return Ok("A fiók sikeresen törölve");
        }

        /// <summary>
        /// A bejelentkezett felhasználó jelszavának módosítása.
        /// </summary>
        /// <param name="dto">A régi és új jelszót tartalmazza.</param>
        /// <returns>200 OK, ha sikeres; 400 vagy 401 hiba esetén.</returns>
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Érvénytelen token");

            var success = await _authService.ChangePasswordAsync(userId, dto);

            if (!success)
                return BadRequest("A jelszó modosítása sikertelen.");

            return Ok("A jelszó sikeresen módosítva.");
        }
    }
}
