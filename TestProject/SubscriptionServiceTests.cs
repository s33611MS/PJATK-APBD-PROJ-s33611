using FluentAssertions;
using Moq;
using PJATK_APBD_PROJ_s33611.DTOs.Subscription;
using PJATK_APBD_PROJ_s33611.Entities;
using PJATK_APBD_PROJ_s33611.Exceptions;
using PJATK_APBD_PROJ_s33611.Repositories.Agreement;
using PJATK_APBD_PROJ_s33611.Repositories.Agreement.Subscription;
using PJATK_APBD_PROJ_s33611.Repositories.Client;
using PJATK_APBD_PROJ_s33611.Repositories.Software;
using PJATK_APBD_PROJ_s33611.Services.Subscription;

namespace TestProject;

public class SubscriptionServiceTests
{
    private readonly Mock<ISubscriptionRepository> _subscriptionRepository = new();
    private readonly Mock<IAgreementRepository> _agreementRepository = new();
    private readonly Mock<IClientRepository> _clientRepository = new();
    private readonly Mock<ISoftwareRepository> _softwareRepository = new();

    private SubscriptionService CreateService()
    {
        return new SubscriptionService(
            _subscriptionRepository.Object,
            _agreementRepository.Object,
            _clientRepository.Object,
            _softwareRepository.Object);
    }
    
    private CreateSubscriptionDto ValidDto()
    {
        return new CreateSubscriptionDto(
            ClientId: 1,
            SoftwareId: 1,
            Name: "Name",
            RenewalPeriodInMonths: 12,
            StartDate: DateOnly.FromDateTime(DateTime.Today)
        );
    }
    
    private readonly Client _client = new IndividualClient
    {
        Id = 1,
        FirstName = "Jan",
        LastName = "Kowalski",
        Pesel = "12345678901",
        Address = "Ulica 1",
        Email = "test@test.pl",
        PhoneNumber = "123456789"
    };
    private readonly Software _software = new()
    {
        Id = 1,
        Name = "Software 1",
        Description = "Software 1 description",
        LicensePricePerYear = 1200,
        SoftwareCategory = new SoftwareCategory
        {
            Id = 1,
            Name = "Software Category"
        }
    };
    
    [Fact]
    public async Task GetById_WhenSubscriptionDoesNotExist_ShouldThrow()
    {
        var service = CreateService();
        
        _subscriptionRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<int>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync((Subscription?)null);
        
        Func<Task> action = async () => await service.GetByIdAsync(1, CancellationToken.None);
        
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task AddAsync_ShouldCreateSubscriptionWithFirstPaymentApplyingDiscount()
    {
        var dto = ValidDto();
        
        var service = CreateService();
        
        _clientRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_client);

        _softwareRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_software);

        _agreementRepository
            .Setup(x => x.HasActiveContractOrSubscriptionForSoftwareAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _agreementRepository
            .Setup(x => x.GetBestDiscountAsync(DiscountType.Subscription, It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        _agreementRepository
            .Setup(x => x.IsReturningClientAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Subscription? saved = null;

        _subscriptionRepository
            .Setup(x => x.AddAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()))
            .Callback<Subscription, CancellationToken>((s, _) =>
            {
                saved = s;
                s.Id = 1;
            });

        _subscriptionRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                saved!.Client = _client;
                saved.Software = _software;
                
                return saved;
            });

        await service.AddAsync(dto, CancellationToken.None);

        saved.Should().NotBeNull();
        saved.Payments.Should().Contain(x => x.Amount == 1020);

        _subscriptionRepository.Verify(x => x.AddAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task AddAsync_WhenRenewalPeriodNotBetween1and24_ShouldThrow()
    {
        var dto = ValidDto() with
        {
            RenewalPeriodInMonths = 0
        };

        var service = CreateService();

        Func<Task> action = async () => await service.AddAsync(dto, CancellationToken.None);

        await action.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("Subscription renewal period must be between 1 month and 2 years");
    }
    
    [Fact]
    public async Task AddAsync_WhenClientDoesNotExist_ShouldThrow()
    {
        var dto = ValidDto();
        
        var service = CreateService();
        
        _clientRepository
            .Setup(x => x.GetByIdAsync(dto.ClientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);

        Func<Task> action = async () => await service.AddAsync(dto, CancellationToken.None);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"There is no Client with id: {dto.ClientId}");
    }
    
    [Fact]
    public async Task AddAsync_WhenSoftwareDoesNotExist_ShouldThrow()
    {
        var dto = ValidDto();
        
        var service = CreateService();
        
        _clientRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IndividualClient());

        _softwareRepository
            .Setup(x => x.GetByIdAsync(dto.SoftwareId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Software?)null);

        Func<Task> action = async () => await service.AddAsync(dto, CancellationToken.None);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"There is no Software with id: {dto.SoftwareId}");
    }
    
    [Fact]
    public async Task AddAsync_WhenActiveAgreementExists_ShouldThrow()
    {
        var dto = ValidDto();
        
        var service = CreateService();
        
        _clientRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IndividualClient());

        _softwareRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Software());

        _agreementRepository
            .Setup(x => x.HasActiveContractOrSubscriptionForSoftwareAsync(1, 1, It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Func<Task> action = async () => await service.AddAsync(dto, CancellationToken.None);

        await action.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage($"Client with id: {dto.ClientId} already has contract or subscription for software with id: {dto.SoftwareId}");
    }
    
    [Fact]
    public async Task AddPayment_ShouldCreatePayment()
    {
        var subscription = new Subscription
        {
            Id = 1,
            ClientId = 1,
            Price = 1000,
            RenewalPeriodInMonths = 1,
            NextRenewalDate = DateOnly.FromDateTime(DateTime.Today)
        };
        
        var service = CreateService();

        _subscriptionRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);

        _agreementRepository
            .Setup(x => x.IsReturningClientAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await service.AddPaymentAsync(new CreateSubscriptionPaymentDto(1, 1000), CancellationToken.None);

        _subscriptionRepository.Verify(x => x.AddPaymentAsync(It.IsAny<SubscriptionPayment>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task AddPayment_ForCancelledSubscription_ShouldThrow()
    {
        var subscription = new Subscription
        {
            Id = 1,
            Status = SubscriptionStatus.Cancelled
        };
        
        var service = CreateService();

        _subscriptionRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);

        Func<Task> action = async () => await service.AddPaymentAsync(new CreateSubscriptionPaymentDto(1, 100), CancellationToken.None);

        await action.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage($"Subscription with id: {subscription.Id} was cancelled");
    }
    
    [Fact]
    public async Task AddPayment_AfterDeadline_ShouldCancelSubscription()
    {
        var subscription = new Subscription
        {
            Id = 1,
            RenewalPeriodInMonths = 1,
            NextRenewalDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-2))
        };
        
        var service = CreateService();

        _subscriptionRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);

        Func<Task> action = async () => await service.AddPaymentAsync(new CreateSubscriptionPaymentDto(1, 100), CancellationToken.None);

        await action.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage("Payment after renewal period, subscription cancelled");

        _subscriptionRepository.Verify(x => x.CancelSubscriptionAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task AddPayment_WhenAlreadyPaid_ShouldThrow()
    {
        var subscription = new Subscription
        {
            Id = 1,
            RenewalPeriodInMonths = 1,
            NextRenewalDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1))
        };
        
        var service = CreateService();
        
        _subscriptionRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);

        Func<Task> action = async () => await service.AddPaymentAsync(new CreateSubscriptionPaymentDto(1, 1), CancellationToken.None);

        await action.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage($"Subscription with id: {subscription.Id} was already paid for");
    }
    
    [Fact]
    public async Task AddPayment_WhenAmountIsInvalid_ShouldThrow()
    {
        var subscription = new Subscription
        {
            Id = 1,
            ClientId = 1,
            Price = 1000,
            RenewalPeriodInMonths = 1,
            NextRenewalDate = DateOnly.FromDateTime(DateTime.Today)
        };

        var payment = new CreateSubscriptionPaymentDto(1, 900);
        
        var service = CreateService();

        _subscriptionRepository
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);

        _agreementRepository
            .Setup(x => x.IsReturningClientAsync(1,It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Func<Task> action = async () => await service.AddPaymentAsync(payment, CancellationToken.None);

        await action.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage($"Subscription costs {subscription.Price}, instead got {payment.Amount}");
    }
}