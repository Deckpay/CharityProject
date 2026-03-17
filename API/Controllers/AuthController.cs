using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}
