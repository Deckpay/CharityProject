using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Moq;

namespace Tests;

/// <summary>
/// LimitService – limit ellenőrzés és felhasználás tesztjei
/// TC-007 hátterének unit szintű lefedése
/// </summary>
public class LimitServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<RequesterLimitRule>> _ruleRepoMock;
    private readonly Mock<IGenericRepository<RequesterLimitUsage>> _usageRepoMock;
    private readonly LimitService _service;

    public LimitServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _ruleRepoMock = new Mock<IGenericRepository<RequesterLimitRule>>();
        _usageRepoMock = new Mock<IGenericRepository<RequesterLimitUsage>>();

        _unitOfWorkMock.Setup(u => u.RequesterLimitRules).Returns(_ruleRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.RequesterLimitUsages).Returns(_usageRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

        _service = new LimitService(_unitOfWorkMock.Object);
    }

    private RequesterLimitRule HaviLimit(int categoryId = 1, int max = 3) => new RequesterLimitRule
    {
        RequesterLimitRuleId = 1,
        RequesterLimitRuleCategoryId = categoryId,
        MaxQuantity = max,
        PeriodType = "monthly",
        IsActive = true
    };

    // ── CanUserRequestProduct ─────────────────────────────────────

    // Nincs aktív limitszabály → mindig engedélyezett
    [Fact]
    public async Task CanUserRequestProduct_NincsAktivSzabaly_Engedelyezett()
    {
        _ruleRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<RequesterLimitRule>());

        var result = await _service.CanUserRequestProduct(userId: 1, categoryId: 1);

        Assert.True(result);
    }

    // Van szabály, de még nem használt → engedélyezett
    [Fact]
    public async Task CanUserRequestProduct_SzabalyVanDeNincsHasznalat_Engedelyezett()
    {
        _ruleRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<RequesterLimitRule> { HaviLimit() });
        _usageRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<RequesterLimitUsage>());

        var result = await _service.CanUserRequestProduct(userId: 1, categoryId: 1);

        Assert.True(result);
    }

    // Felhasználó elérte a maximumot → tiltott
    [Fact]
    public async Task CanUserRequestProduct_LimitElerve_Tiltott()
    {
        var rule = HaviLimit(max: 2);
        var now = DateTime.UtcNow;
        var usage = new RequesterLimitUsage
        {
            RequesterId = 1,
            RuleId = rule.RequesterLimitRuleId,
            UsedQuantity = 2, // == MaxQuantity
            PeriodStart = new DateTime(now.Year, now.Month, 1),
            PeriodEnd = new DateTime(now.Year, now.Month, 1).AddMonths(1)
        };

        _ruleRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<RequesterLimitRule> { rule });
        _usageRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<RequesterLimitUsage> { usage });

        var result = await _service.CanUserRequestProduct(userId: 1, categoryId: 1);

        Assert.False(result);
    }

    // ── TryConsumeLimit ───────────────────────────────────────────

    // Első igénylés a periódusban → új Usage rekord jön létre
    [Fact]
    public async Task TryConsumeLimit_ElsoIgenyles_UjUsageLetrejon()
    {
        _ruleRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<RequesterLimitRule> { HaviLimit() });
        _usageRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<RequesterLimitUsage>());

        var result = await _service.TryConsumeLimit(userId: 1, categoryId: 1);

        Assert.True(result);
        _usageRepoMock.Verify(r => r.AddAsync(It.IsAny<RequesterLimitUsage>()), Times.Once);
    }

    // Limit túllépési kísérlet → visszautasítva, Usage nem változik
    [Fact]
    public async Task TryConsumeLimit_LimitTullep_Visszautasitva()
    {
        var rule = HaviLimit(max: 1);
        var now = DateTime.UtcNow;
        var usage = new RequesterLimitUsage
        {
            RequesterId = 1,
            RuleId = rule.RequesterLimitRuleId,
            UsedQuantity = 1, // == MaxQuantity
            PeriodStart = new DateTime(now.Year, now.Month, 1),
            PeriodEnd = new DateTime(now.Year, now.Month, 1).AddMonths(1)
        };

        _ruleRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<RequesterLimitRule> { rule });
        _usageRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<RequesterLimitUsage> { usage });

        var result = await _service.TryConsumeLimit(userId: 1, categoryId: 1);

        Assert.False(result);
    }

    // ── DecreaseLimitUsage ────────────────────────────────────────

    // Igénylés visszavonásakor a számláló csökken
    [Fact]
    public async Task DecreaseLimitUsage_VanAktivHasznalat_CsokkentSzamlalot()
    {
        var rule = HaviLimit();
        var now = DateTime.UtcNow;
        var usage = new RequesterLimitUsage
        {
            RequesterId = 1,
            RuleId = rule.RequesterLimitRuleId,
            UsedQuantity = 2,
            PeriodStart = now.AddHours(-1),
            PeriodEnd = now.AddDays(29)
        };

        _ruleRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<RequesterLimitRule> { rule });
        _usageRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<RequesterLimitUsage> { usage });

        var result = await _service.DecreaseLimitUsage(userId: 1, categoryId: 1);

        Assert.True(result);
        Assert.Equal(1, usage.UsedQuantity);
    }

    // Nincs aktív használat → false
    [Fact]
    public async Task DecreaseLimitUsage_NincsHasznalat_Sikertelen()
    {
        _ruleRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<RequesterLimitRule> { HaviLimit() });
        _usageRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<RequesterLimitUsage>());

        var result = await _service.DecreaseLimitUsage(userId: 1, categoryId: 1);

        Assert.False(result);
    }
}