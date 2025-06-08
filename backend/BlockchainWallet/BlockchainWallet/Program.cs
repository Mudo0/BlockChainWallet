using BlockchainWallet.Extensions;
using BlockchainWallet.Repositories;
using Microsoft.AspNetCore.DataProtection.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//llama al m�todo de extensi�n para configurar la conexi�n a Neo4j
builder.Services.AddDbConfiguration(builder.Configuration)
    .AddDrivers(builder.Configuration);

builder.Services.AddScoped<IDataAccess, DataAccess>();
builder.Services.AddTransient<IRepository, Repository>();

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