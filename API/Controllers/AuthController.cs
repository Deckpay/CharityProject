using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;
using Infrastructure.Repositories;
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
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        // kontroller bekéri az autservice-t

        public AuthController(IAuthService authService,JwtTokenGenerator jwtTokenGenerator)
        {
            _authService = authService;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        // rész automatikusan behelyettesíti az osztály nevét, levágva a "Controller" szót
        [HttpPost("register")] // Ez lesz a végpont: POST api/Auth/register
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // 1. A kontroller csak továbbpasszolja a labdát a servicenek
            var success = await _authService.RegisterAsync(dto);

            // 2. Visszaszól a Web-nek, hogy sikerűlt e
            if (success) return Ok();
            return BadRequest("A regisztráció sikertelen");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _authService.LoginAsync(dto.EmailOrUserName, dto.Password);

            if (user == null)
                return Unauthorized();

            if (user.UserStatus != UserStatus.Active)
                return Unauthorized("User not active");

            var token = _jwtTokenGenerator.GenerateToken(user);

            return Ok(new LoginResponseDto { Token = token });
        }

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
