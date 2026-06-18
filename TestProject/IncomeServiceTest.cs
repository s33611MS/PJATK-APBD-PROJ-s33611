using FluentAssertions;
using Moq;
using PJATK_APBD_PROJ_s33611.DTOs.Currency;
using PJATK_APBD_PROJ_s33611.Entities;
using PJATK_APBD_PROJ_s33611.Exceptions;
using PJATK_APBD_PROJ_s33611.Repositories.Income;
using PJATK_APBD_PROJ_s33611.Repositories.Software;
using PJATK_APBD_PROJ_s33611.Services.Income;

namespace TestProject;

public class IncomeServiceTest
{
    private readonly Mock<IIncomeRepository> _incomeRepository = new();
    private readonly Mock<ISoftwareRepository> _softwareRepository = new();
    private readonly Mock<ICurrencyService> _currencyService = new();
    private readonly Mock<ICurrencyRepository> _currencyRepository = new();
    
    private IncomeService CreateService()
    {
        return new IncomeService(
            _incomeRepository.Object,
            _currencyService.Object,
            _softwareRepository.Object
        );
    }
    
    [Fact]
    public async Task GetAsync_WhenCurrencyIsNull_ShouldUsePln()
    {
        var service = CreateService();
        
        _incomeRepository
            .Setup(x => x.GetAsync(null, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1000);

        _currencyService
            .Setup(x => x.GetExchangeRateAsync("PLN", It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        var result = await service.GetAsync(null, null, false, CancellationToken.None);
        
        result.Income.Should().Be(1000);
        
        _currencyService.Verify(x => x.GetExchangeRateAsync("PLN", It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task GetAsync_ShouldConvertCurrency()
    {
        _incomeRepository
            .Setup(x => x.GetAsync(null, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1000);

        _currencyService
            .Setup(x => x.GetExchangeRateAsync("EUR", It.IsAny<CancellationToken>()))
            .ReturnsAsync(4);

        var service = CreateService();
        
        var result = await service.GetAsync(null, "EUR", false, CancellationToken.None);
        
        result.Income.Should().Be(250);
        result.CurrencyCode.Should().Be("EUR");
        result.ExchangeRate.Should().Be(4);
    }
    
    [Fact]
    public async Task GetAsync_ShouldPassSoftwareIdToRepository()
    {
        var service = CreateService();

        _softwareRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Software { Id = 1 });

        _incomeRepository
            .Setup(x => x.GetAsync(1, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(100);

        _currencyService
            .Setup(x => x.GetExchangeRateAsync("PLN", It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        var result = await service.GetAsync(1, null, false, CancellationToken.None);
        
        result.Income.Should().Be(100);
        
        _incomeRepository.Verify(x => x.GetAsync(1, false, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task GetAsync_WhenSoftwareDoesNotExist_ShouldThrow()
    {
        const int softwareId = 1;
        
        var service = CreateService();
        
        _softwareRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Software?)null);
        
        Func<Task> action = () => service.GetAsync(softwareId, null, false, CancellationToken.None);
        
        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"There is no software with id {softwareId}");
    }
    
    [Fact]
    public async Task GetAsync_WhenCurrencyDoesNotExist_ShouldThrow()
    {
        const string code = "NON";
        
        var service = new CurrencyService(_currencyRepository.Object);
        
        _currencyRepository
            .Setup(x => x.GetExchangeRateAsync(code, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NbpResponseDto?)null);
        
        Func<Task> action = () => service.GetExchangeRateAsync(code, CancellationToken.None);
        
        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Currency with code: {code} was not found");
    }
}