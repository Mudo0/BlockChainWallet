using BlockchainWallet.DataAccess;
using BlockchainWallet.Extensions;
using BlockchainWallet.Persistence;
using BlockchainWallet.Repositories;
using Microsoft.AspNetCore.DataProtection.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//llama al método de extensión para configurar la conexión a Neo4j
builder.Services.AddDbConfiguration(builder.Configuration);

builder.Services.AddScoped<IDataAccess, DataAccess>();
builder.Services.AddTransient<BlockRepository>();

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