using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace Tests;

/// <summary>
/// TC-001, TC-002, TC-003, TC-004 – Regisztráció és bejelentkezés tesztjei
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<User>> _userRepoMock;
    private readonly Mock<IJwtTokenGenerator> _jwtMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepoMock = new Mock<IGenericRepository<User>>();
        _jwtMock = new Mock<IJwtTokenGenerator>();

        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepoMock.Object);

        _authService = new AuthService(_unitOfWorkMock.Object, _jwtMock.Object);
    }

    // ────────────────────────────────────────────────────────────
    // TC-001 – Regisztráció érvényes adatokkal → sikeres
    // ────────────────────────────────────────────────────────────
    [Fact]
    public async Task TC001_RegisterAsync_ErvenyesAdatok_SikeresRegisztracio()
    {
        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());
        _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

        var dto = new RegisterDto
        {
            UserName = "ujfelhasznalo",
            Email = "uj@teszt.com",
            FirstName = "Teszt",
            LastName = "Elek",
            Password = "Jelszo123!",
            RoleId = (int)UserRole.Sender
        };

        var result = await _authService.RegisterAsync(dto);

        Assert.True(result);
    }

    // ────────────────────────────────────────────────────────────
    // TC-002 – Regisztráció már létező e-maillel → sikertelen
    // ────────────────────────────────────────────────────────────
    [Fact]
    public async Task TC002_RegisterAsync_FoglaltEmail_SikertelenRegisztracio()
    {
        var meglevoUser = new User { Email = "foglalt@teszt.com", UserName = "valaki" };
        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User> { meglevoUser });

        var dto = new RegisterDto
        {
            UserName = "masvalaki",
            Email = "foglalt@teszt.com",
            Password = "Jelszo123!"
        };

        var result = await _authService.RegisterAsync(dto);

        Assert.False(result);
    }

    // TC-002b – Regisztráció már létező felhasználónévvel → sikertelen
    [Fact]
    public async Task TC002b_RegisterAsync_FoglaltUserName_SikertelenRegisztracio()
    {
        var meglevoUser = new User { Email = "mas@teszt.com", UserName = "foglalt_nev" };
        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User> { meglevoUser });

        var dto = new RegisterDto
        {
            UserName = "foglalt_nev",
            Email = "uj@teszt.com",
            Password = "Jelszo123!"
        };

        var result = await _authService.RegisterAsync(dto);

        Assert.False(result);
    }

    // ────────────────────────────────────────────────────────────
    // TC-003 – Bejelentkezés helyes adatokkal → JWT token érkezik
    // ────────────────────────────────────────────────────────────
    [Fact]
    public async Task TC003_LoginAsync_HelyesAdatok_TokentAd()
    {
        const string jelszo = "123123";
        var user = new User
        {
            Email = "userSender@teszt.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(jelszo),
            UserStatus = UserStatus.Active
        };
        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User> { user });
        _jwtMock.Setup(j => j.GenerateToken(user)).Returns("teszt.jwt.token");

        var result = await _authService.LoginAsync("userSender@teszt.com", jelszo);

        Assert.NotNull(result?.Token);
        Assert.True(string.IsNullOrEmpty(result?.ErrorMessage));
    }

    // ────────────────────────────────────────────────────────────
    // TC-004 – Bejelentkezés helytelen jelszóval → hibaüzenet
    // ────────────────────────────────────────────────────────────
    [Fact]
    public async Task TC004_LoginAsync_HelytelenJelszo_HibauzenettelTer()
    {
        var user = new User
        {
            Email = "userSender@teszt.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123123"),
            UserStatus = UserStatus.Active
        };
        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User> { user });

        var result = await _authService.LoginAsync("userSender@teszt.com", "RossszJelszo");

        Assert.NotNull(result?.ErrorMessage);
        Assert.True(string.IsNullOrEmpty(result?.Token));
    }

    // ── Határesetek (2.5.4 alapján) ──────────────────────────────

    // Tiltott fiók bejelentkezési kísérlete → hibaüzenet
    [Fact]
    public async Task LoginAsync_TiltottFiok_Hibauzenettel()
    {
        const string jelszo = "Jelszo123!";
        var user = new User
        {
            Email = "tiltott@teszt.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(jelszo),
            UserStatus = UserStatus.Banned
        };
        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User> { user });

        var result = await _authService.LoginAsync("tiltott@teszt.com", jelszo);

        Assert.Contains("tiltva", result?.ErrorMessage);
    }

    // Törölt fiók bejelentkezési kísérlete → hibaüzenet
    [Fact]
    public async Task LoginAsync_ToroltFiok_Hibauzenettel()
    {
        const string jelszo = "Jelszo123!";
        var user = new User
        {
            Email = "torolt@teszt.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(jelszo),
            UserStatus = UserStatus.Deleted
        };
        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User> { user });

        var result = await _authService.LoginAsync("torolt@teszt.com", jelszo);

        Assert.Contains("törölve", result?.ErrorMessage);
    }

    // Nem létező felhasználóval való bejelentkezés → hibaüzenet
    [Fact]
    public async Task LoginAsync_NemLetezoFelhasznalo_Hibauzenettel()
    {
        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());

        var result = await _authService.LoginAsync("senki@teszt.com", "Jelszo123!");

        Assert.NotNull(result?.ErrorMessage);
    }

    // ── Jelszóváltoztatás tesztjei ────────────────────────────────

    // Sikeres jelszóváltoztatás
    [Fact]
    public async Task ChangePasswordAsync_ErvenyesAdatok_Sikeres()
    {
        const string regiJelszo = "Regi123!";
        var user = new User { UserId = 1, PasswordHash = BCrypt.Net.BCrypt.HashPassword(regiJelszo) };
        _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

        var dto = new ChangePasswordDto
        {
            CurrentPassword = "Regi123!",
            NewPassword = "Uj123!",
            ConfirmNewPassword = "Uj123!"
        };

        var result = await _authService.ChangePasswordAsync(1, dto);

        Assert.True(result);
    }

    // Új és régi jelszó megegyezik → sikertelen
    [Fact]
    public async Task ChangePasswordAsync_UjEsRegiMegegyezik_Sikertelen()
    {
        var user = new User { UserId = 1, PasswordHash = BCrypt.Net.BCrypt.HashPassword("Regi123!") };
        _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        var dto = new ChangePasswordDto
        {
            CurrentPassword = "Regi123!",
            NewPassword = "Regi123!",
            ConfirmNewPassword = "Regi123!"
        };

        var result = await _authService.ChangePasswordAsync(1, dto);

        Assert.False(result);
    }

    // Megerősítő jelszó nem egyezik → sikertelen
    [Fact]
    public async Task ChangePasswordAsync_MegerositesNemEgyezik_Sikertelen()
    {
        var user = new User { UserId = 1, PasswordHash = BCrypt.Net.BCrypt.HashPassword("Regi123!") };
        _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        var dto = new ChangePasswordDto
        {
            CurrentPassword = "Regi123!",
            NewPassword = "Uj123!",
            ConfirmNewPassword = "MasUj456!"
        };

        var result = await _authService.ChangePasswordAsync(1, dto);

        Assert.False(result);
    }
}