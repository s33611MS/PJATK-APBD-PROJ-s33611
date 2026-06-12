using Microsoft.AspNetCore.Mvc;
using PJATK_APBD_PROJ_s33611.DTOs.Client;
using PJATK_APBD_PROJ_s33611.Services;

namespace PJATK_APBD_PROJ_s33611.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController(IClientService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await service.GetAllAsync(cancellationToken));
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        return Ok(await service.GetByIdAsync(id, cancellationToken));
    }
    
    [HttpPost("individual")]
    public async Task<IActionResult> AddIndividual([FromBody] CreateIndividualClientDto request, CancellationToken cancellationToken)
    {
        var id = await service.AddIndividualAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }
    
    [HttpPost("company")]
    public async Task<IActionResult> AddCompany([FromBody] CreateCompanyClientDto request, CancellationToken cancellationToken)
    {
        var id = await service.AddCompanyAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("individual/{id:int}")]
    public async Task<IActionResult> UpdateIndividual([FromRoute] int id, [FromBody] UpdateIndividualClientDto request, CancellationToken cancellationToken)
    {
        await service.UpdateIndividualAsync(id, request, cancellationToken);
        return NoContent();
    }
    
    [HttpPut("company/{id:int}")]
    public async Task<IActionResult> UpdateCompany([FromRoute] int id, [FromBody] UpdateCompanyClientDto request, CancellationToken cancellationToken)
    {
        await service.UpdateCompanyAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}