using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace Tests;

/// <summary>
/// TC-006, TC-007, TC-008 – Termékigénylés tesztjei
/// </summary>
public class ProductRequestServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<Product>> _productRepoMock;
    private readonly Mock<IGenericRepository<ProductRequest>> _requestRepoMock;
    private readonly Mock<ILimitService> _limitServiceMock;
    private readonly Mock<IDbContextTransaction> _transactionMock;
    private readonly ProductRequestService _service;

    public ProductRequestServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepoMock = new Mock<IGenericRepository<Product>>();
        _requestRepoMock = new Mock<IGenericRepository<ProductRequest>>();
        _limitServiceMock = new Mock<ILimitService>();
        _transactionMock = new Mock<IDbContextTransaction>(); // ← javítva

        _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.ProductRequests).Returns(_requestRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync())
            .Returns(Task.FromResult(_transactionMock.Object)); // ← javítva
        _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

        _service = new ProductRequestService(_unitOfWorkMock.Object, _limitServiceMock.Object);
    }

    private Product AktivTermek(int productId = 1, int senderId = 99, int categoryId = 1) => new Product
    {
        ProductId = productId,
        SenderId = senderId,
        ProductCategoryId = categoryId,
        ProductStatus = ProductStatus.Active
    };

    // ────────────────────────────────────────────────────────────
    // TC-006 – Sikeres igénylés (limit alatt)
    // ────────────────────────────────────────────────────────────
    [Fact]
    public async Task TC006_ClaimProductAsync_LimitAlatt_SikeresIgenyles()
    {
        var product = AktivTermek();
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _requestRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ProductRequest>());
        _limitServiceMock.Setup(l => l.CanUserRequestProduct(2, 1)).ReturnsAsync(true);
        _limitServiceMock.Setup(l => l.UpdateLimitUsage(2, 1)).ReturnsAsync(true);

        var result = await _service.ClaimProductAsync(productId: 1, userId: 2);

        Assert.True(result.Success);
    }

    // ────────────────────────────────────────────────────────────
    // TC-007 – Igénylés megtagadva (limit elérve)
    // ────────────────────────────────────────────────────────────
    [Fact]
    public async Task TC007_ClaimProductAsync_LimitElerve_Megtagadva()
    {
        var product = AktivTermek();
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _limitServiceMock.Setup(l => l.CanUserRequestProduct(2, 1)).ReturnsAsync(false);

        var result = await _service.ClaimProductAsync(productId: 1, userId: 2);

        Assert.False(result.Success);
        Assert.Contains("limit", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ────────────────────────────────────────────────────────────
    // TC-008 – Már igényelt (Pending státuszú) termék igénylése
    // ────────────────────────────────────────────────────────────
    [Fact]
    public async Task TC008_ClaimProductAsync_MarFoglaltTermek_Megtagadva()
    {
        var product = AktivTermek();
        var meglevoIgnyles = new ProductRequest
        {
            ProductId = 1,
            RequesterId = 5,
            RequestStatus = RequestStatus.Pending
        };

        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _limitServiceMock.Setup(l => l.CanUserRequestProduct(2, 1)).ReturnsAsync(true);
        _limitServiceMock.Setup(l => l.UpdateLimitUsage(2, 1)).ReturnsAsync(true);
        _requestRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<ProductRequest> { meglevoIgnyles });

        var result = await _service.ClaimProductAsync(productId: 1, userId: 2);

        Assert.False(result.Success);
        Assert.Contains("foglalt", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ── Határesetek ───────────────────────────────────────────────

    // Nem létező termék igénylése
    [Fact]
    public async Task ClaimProductAsync_NemLetezoTermek_Sikertelen()
    {
        _productRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        var result = await _service.ClaimProductAsync(productId: 99, userId: 2);

        Assert.False(result.Success);
        Assert.Contains("nem található", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    // Saját termék igénylése → tiltott
    [Fact]
    public async Task ClaimProductAsync_SajatTermek_Megtagadva()
    {
        var product = AktivTermek(senderId: 2);
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        var result = await _service.ClaimProductAsync(productId: 1, userId: 2);

        Assert.False(result.Success);
        Assert.Contains("saját", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    // Nem aktív termék igénylése
    [Fact]
    public async Task ClaimProductAsync_NemAktivTermek_Megtagadva()
    {
        var product = AktivTermek();
        product.ProductStatus = ProductStatus.Completed;
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        var result = await _service.ClaimProductAsync(productId: 1, userId: 2);

        Assert.False(result.Success);
        Assert.Contains("nem igényelhető", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ── Igénylés törlése ──────────────────────────────────────────

    // Sikeres igénylés-visszavonás
    [Fact]
    public async Task DeleteRequestAsync_SajatIgenyles_Sikeres()
    {
        var request = new ProductRequest
        {
            ProductRequestId = 1,
            RequesterId = 2,
            ProductId = 1,
            RequestStatus = RequestStatus.Pending
        };
        var product = AktivTermek();

        _requestRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(request);
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

        var result = await _service.DeleteRequestAsync(requestId: 1, userId: 2);

        Assert.True(result);
    }

    // Más felhasználó igénylésének törlési kísérlete
    [Fact]
    public async Task DeleteRequestAsync_MasFelhasznaloIgenylese_Sikertelen()
    {
        var request = new ProductRequest
        {
            ProductRequestId = 1,
            RequesterId = 99,
            ProductId = 1,
            RequestStatus = RequestStatus.Pending
        };
        _requestRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(request);

        var result = await _service.DeleteRequestAsync(requestId: 1, userId: 2);

        Assert.False(result);
    }

    // ── Igénylés lezárása (adományozó részéről) ───────────────────

    // Sikeres lezárás (success = true)
    [Fact]
    public async Task CompleteRequestAsync_Sikeres_TermekCompleted()
    {
        var request = new ProductRequest
        {
            ProductRequestId = 1,
            RequesterId = 3,
            ProductId = 1,
            RequestStatus = RequestStatus.Pending
        };
        var product = AktivTermek(senderId: 2);

        _requestRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(request);
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

        var result = await _service.CompleteRequestAsync(requestId: 1, userId: 2, success: true);

        Assert.True(result);
        Assert.Equal(RequestStatus.Completed, request.RequestStatus);
        Assert.Equal(ProductStatus.Completed, product.ProductStatus);
    }

    // Elutasítás (success = false) → termék visszaáll Active-ra
    [Fact]
    public async Task CompleteRequestAsync_Elutasitas_TermekActiveAll()
    {
        var request = new ProductRequest
        {
            ProductRequestId = 1,
            RequesterId = 3,
            ProductId = 1,
            RequestStatus = RequestStatus.Pending
        };
        var product = AktivTermek(senderId: 2);
        product.ProductStatus = ProductStatus.Pending;

        _requestRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(request);
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

        var result = await _service.CompleteRequestAsync(requestId: 1, userId: 2, success: false);

        Assert.True(result);
        Assert.Equal(RequestStatus.Failed, request.RequestStatus);
        Assert.Equal(ProductStatus.Active, product.ProductStatus);
    }
}