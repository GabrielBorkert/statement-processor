using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/extratos", async (string conteudoExtrato) =>
{
    var factory = new ConnectionFactory { HostName = "localhost" };

    using var connection = await factory.CreateConnectionAsync();
    using var channel = await connection.CreateChannelAsync();

    await channel.QueueDeclareAsync(queue: "extratos", durable: true, exclusive: false, autoDelete: false, arguments: null);

    var body = Encoding.UTF8.GetBytes(conteudoExtrato);

    await channel.BasicPublishAsync(exchange: "", routingKey: "extratos", body: body);

    return Results.Accepted();
});

app.Run();