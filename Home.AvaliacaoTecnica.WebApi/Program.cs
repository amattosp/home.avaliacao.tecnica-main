using Home.AvaliacaoTecnica.Application;
using Home.AvaliacaoTecnica.Application.Config;
using Home.AvaliacaoTecnica.Application.Services.Wrapper;
using Home.AvaliacaoTecnica.Domain.Interfaces;
using Home.AvaliacaoTecnica.Infra.Data.Context;
using Home.AvaliacaoTecnica.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.ApplicationInsights(
        builder.Configuration["ApplicationInsights:ConnectionString"],
        TelemetryConverter.Traces)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true; // Optional, for pretty formatting
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

var assemblies = new[] { typeof(Program).Assembly, typeof(ApplicationLayer).Assembly };
builder.Services.AddAutoMapper(assemblies);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(assemblies)
);

//Configuracao do Azure Service Bus
builder.Services.AddTransient<IServiceBusSenderWrapper, ServiceBusSenderWrapper>();
builder.Services.AddAzureServiceBusConfiguration(
    sbBuilder => sbBuilder
                        .AddSender(
                            "TopicSender",
                            "pedidos")
                        .AddProcessor(
                            "TopicProcessor",
                            "pedidos",
                            "processado"));

builder.Services.AddSingleton(Log.Logger);
builder.Services.AddTransient<IPedidoRepository, PedidoRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }