using Microsoft.EntityFrameworkCore;
using PJATK_APBD_PROJ_s33611.Data;
using PJATK_APBD_PROJ_s33611.Exceptions;
using PJATK_APBD_PROJ_s33611.Repositories;
using PJATK_APBD_PROJ_s33611.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IClientRepository, ClientRepository>();

builder.Services.AddScoped<IClientService, ClientService>();

builder.Services.AddDbContext<DatabaseContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opt => opt.SwaggerEndpoint("/openapi/v1.json", "V1"));
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();