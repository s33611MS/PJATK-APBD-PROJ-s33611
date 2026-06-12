using PJATK_APBD_PROJ_s33611.DTOs;
using PJATK_APBD_PROJ_s33611.DTOs.Client;
using PJATK_APBD_PROJ_s33611.Entities;
using PJATK_APBD_PROJ_s33611.Exceptions;
using PJATK_APBD_PROJ_s33611.Repositories;

namespace PJATK_APBD_PROJ_s33611.Services;

public class ClientService(IClientRepository repo) : IClientService
{
    public async Task<IEnumerable<ClientResponseDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var clients = await repo.GetAllAsync(cancellationToken);

        return clients
            .Select(MapToDto)
            .ToList();
    }

    public async Task<ClientResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var client = await repo.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException($"There is no Client with id: {id}");

        return MapToDto(client);
    }

    public async Task<int> AddIndividualAsync(CreateIndividualClientDto dto, CancellationToken cancellationToken)
    {
        if(await repo.PeselExistsAsync(dto.Pesel, cancellationToken))
            throw new ConflictException($"There already is a client with Pesel: {dto.Pesel}");
        
        var client = new IndividualClient{
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Pesel = dto.Pesel,
            Address = dto.Address,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber
        };

        await repo.AddAsync(client, cancellationToken);

        return client.Id;
    }

    public async Task<int> AddCompanyAsync(CreateCompanyClientDto dto, CancellationToken cancellationToken)
    {
        if(await repo.KrsExistsAsync(dto.Krs, cancellationToken))
            throw new ConflictException($"There already is a client with KRS: {dto.Krs}");
        
        var client = new CompanyClient{
            CompanyName = dto.CompanyName,
            Krs = dto.Krs,
            Address = dto.Address,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber
        };

        await repo.AddAsync(client, cancellationToken);
        
        return client.Id;
    }

    public async Task UpdateIndividualAsync(int id, UpdateIndividualClientDto dto, CancellationToken cancellationToken)
    {
        var client = await repo.GetIndividualByIdAsync(id, cancellationToken) ?? throw new NotFoundException($"There is no Individual Client with id: {id}");

        client.Address = dto.Address;
        client.Email = dto.Email;
        client.PhoneNumber = dto.PhoneNumber;
        client.FirstName = dto.FirstName;
        client.LastName = dto.LastName;

        await repo.UpdateAsync(client, cancellationToken);
    }

    public async Task UpdateCompanyAsync(int id, UpdateCompanyClientDto dto, CancellationToken cancellationToken)
    {
        var client = await repo.GetCompanyByIdAsync(id, cancellationToken) ?? throw new NotFoundException($"There is no Company Client with id: {id}");

        client.Address = dto.Address;
        client.Email = dto.Email;
        client.PhoneNumber = dto.PhoneNumber;
        client.CompanyName = dto.CompanyName;

        await repo.UpdateAsync(client, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var client = await repo.GetByIdAsync(id, cancellationToken) ??  throw new NotFoundException($"There is no Client with id: {id}");
        switch (client)
        {
            case IndividualClient individual:
                individual.IsDeleted = true;
                await repo.UpdateAsync(individual, cancellationToken);
                break;
            case CompanyClient:
                throw new ConflictException("You can't delete companies");
        }
    }
    
    private static ClientResponseDto MapToDto(Client client)
    {
        return client switch
        {
            IndividualClient individual => new IndividualClientResponseDto
            (
                individual.Id,
                individual.Address,
                individual.Email,
                individual.PhoneNumber,
                individual.FirstName,
                individual.LastName,
                individual.Pesel
            ),

            CompanyClient company => new CompanyClientResponseDto
            (
                company.Id,
                company.Address,
                company.Email,
                company.PhoneNumber,
                company.CompanyName,
                company.Krs
            ),

            _ => throw new InvalidOperationException()
        };
    }
}