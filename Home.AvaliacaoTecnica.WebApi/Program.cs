using Home.AvaliacaoTecnica.Infra.Data.Context;
using Home.AvaliacaoTecnica.Infra.Data.Repositories;
using Home.AvalicaoTecnica.WebApi.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

//todo: Configurar Application Insights integrador com Serilog
builder.Host.UseSerilog();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true; // opcional, para formatar bonito
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddTransient<PedidoRepository>();
builder.Services.AddDbContext<PedidoDbContext>(options =>
    options.UseInMemoryDatabase("PedidosDb"));

// Configuração do Azure Service Bus
builder.Services.AddSingleton<ServiceBusSenderFactory>();
builder.Services.AddSingleton(Log.Logger); 

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();

