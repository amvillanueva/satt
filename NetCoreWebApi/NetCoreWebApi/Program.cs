global using NetCoreWebApi.Data;
global using Microsoft.EntityFrameworkCore;
using Application.UseCases.Acopio.Movil.Implementacion;
using Application.UseCases.Acopio.Movil;
using Application.Repositories.Plataforma;
using Infrastructure.DataAccess.Plataforma;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//instancia de base de datos
builder.Services.AddDbContext<DataContext>(options =>{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefatultConnection"));


});


// Registrar la implementación de IAcopioUseCase
builder.Services.AddScoped<IAcopioUseCase, AcopioUseCase>();

builder.Services.AddScoped<IAcopioRepository, AcopioRepository>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefatultConnection");
    return new AcopioRepository(connectionString);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
