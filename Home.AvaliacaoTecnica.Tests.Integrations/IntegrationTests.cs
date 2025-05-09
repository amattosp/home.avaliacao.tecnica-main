using FluentAssertions;
using Home.AvaliacaoTecnica.Application.Pedido.ListarPorId;
using Home.AvaliacaoTecnica.Domain.Entities;
using Home.AvaliacaoTecnica.Infra.Data.Context;
using Home.AvaliacaoTecnica.Tests.Integrations.DataBuilders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly MsSqlContainer _sqlContainer;
    
    private async Task ResetDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PedidoDbContext>();

        context.Pedidos.RemoveRange(context.Pedidos);
        await context.SaveChangesAsync();
    }

    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _sqlContainer = new MsSqlBuilder()
            .WithPassword("yourStrong(!)Password")
            .Build();

        _sqlContainer.StartAsync().Wait();

        
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task Pedido_QuandoConsultarPedidoPeloId_DeveRetornarUmPedidoUnicoComItens()
    {
        // Arrange
        await ResetDatabase(); 
        var client = CreateHttpClient();
        var pedidoEnviado = await SeedDatabaseWithSinglePedido();

        // Act
        var response = await client.GetAsync($"/api/pedidos/{pedidoEnviado.Id}");
        var pedidoRetornado = await DeserializeResponse<PedidoResponse>(response);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK, "O endpoint deve retornar 200 OK");
        pedidoRetornado.Should().NotBeNull();
        pedidoRetornado!.Id.Should().Be(pedidoEnviado.Id);
        pedidoRetornado.PedidoId.Should().Be(pedidoEnviado.PedidoId);
        pedidoRetornado.ClienteId.Should().Be(pedidoEnviado.ClienteId);
        pedidoRetornado.Status.Should().Be(pedidoEnviado.Status);
        pedidoRetornado.Itens.Should().NotBeEmpty();
        await ResetDatabase();
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task Pedido_QuandoConsultarPedidosPeloStatus_DeveRetornarListaDePedidosComItens()
    {
        // Arrange
        var client = CreateHttpClient();
        var pedidosEnviados = await SeedDatabaseWithPedidosByStatus("Criado", 2);

        // Act
        var response = await client.GetAsync($"/api/pedidos?status=Criado");
        var pedidosRetornados = await DeserializeResponse<List<PedidoResponse>>(response);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK, "O endpoint deve retornar 200 OK");
        pedidosRetornados.Should().NotBeNull();
        pedidosRetornados.Should().NotBeEmpty();
    }

    private HttpClient CreateHttpClient()
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddDbContext<PedidoDbContext>(options =>
                    options.UseSqlServer(_sqlContainer.GetConnectionString()));
            });
        }).CreateClient();
    }

    private async Task<PedidoEnviado> SeedDatabaseWithSinglePedido()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PedidoDbContext>();
        var pedidosEnviados = new PedidoDataBuilder().ComPedidoId(1).BuildList(1);
        var pedidoEnviado = pedidosEnviados.First();
        context.Pedidos.AddRange(pedidosEnviados);
        await context.SaveChangesAsync();
        return pedidoEnviado;
    }

    private async Task<List<PedidoEnviado>> SeedDatabaseWithPedidosByStatus(string status, int count)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PedidoDbContext>();

        var pedidosEnviados = Enumerable.Range(1, count)
            .Select(id => new PedidoDataBuilder()
                .ComId(id)
                .ComStatus(status)
                .Build())
            .ToList();

        context.Pedidos.AddRange(pedidosEnviados);
        await context.SaveChangesAsync();
        return pedidosEnviados;
    }


    private async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        return System.Text.Json.JsonSerializer.Deserialize<T>(responseContent, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }
}