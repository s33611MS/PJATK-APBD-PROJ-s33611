using PJATK_APBD_PROJ_s33611.DTOs.Client;
using PJATK_APBD_PROJ_s33611.Entities;
using PJATK_APBD_PROJ_s33611.Exceptions;
using PJATK_APBD_PROJ_s33611.Mappers;
using PJATK_APBD_PROJ_s33611.Repositories;

namespace PJATK_APBD_PROJ_s33611.Services;

public class ClientService(IClientRepository repo) : IClientService
{
    public async Task<IEnumerable<ClientResponseDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var clients = await repo.GetAllAsync(cancellationToken);

        return clients
            .Select(ClientMapper.ToDto)
            .ToList();
    }

    public async Task<ClientResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var client = await repo.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException($"There is no Client with id: {id}");

        return ClientMapper.ToDto(client);
    }

    public async Task<ClientResponseDto> AddAsync(CreateClientDto dto, CancellationToken cancellationToken)
    {
        switch (dto)
        {
            case CreateIndividualClientDto individual:
                if(await repo.PeselExistsAsync(individual.Pesel, cancellationToken))
                    throw new ConflictException($"There already is a client with Pesel: {individual.Pesel}");
                break;
            case CreateCompanyClientDto company:
                if(await repo.KrsExistsAsync(company.Krs, cancellationToken))
                    throw new ConflictException($"There already is a client with KRS: {company.Krs}");
                break;
        }

        var client = ClientMapper.ToEntity(dto);

        await repo.AddAsync(client, cancellationToken);

        return ClientMapper.ToDto(client);
    }

    public async Task UpdateAsync(int id, UpdateClientDto dto, CancellationToken cancellationToken)
    {
        var client = await repo.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException($"There is no Client with id: {id}");
        
        switch (client, dto)
        {
            case (IndividualClient individual, UpdateIndividualClientDto individualDto):
                individual.Address = individualDto.Address;
                individual.Email = individualDto.Email;
                individual.PhoneNumber = individualDto.PhoneNumber;
                individual.FirstName = individualDto.FirstName;
                individual.LastName = individualDto.LastName;
                break;
            case (CompanyClient company, UpdateCompanyClientDto companyDto):
                company.Address = companyDto.Address;
                company.Email = companyDto.Email;
                company.PhoneNumber = companyDto.PhoneNumber;
                company.CompanyName = companyDto.CompanyName;
                break;
            default:
                throw new ConflictException($"Client with id: {id} is of different type than requested body");
        }
        
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
}