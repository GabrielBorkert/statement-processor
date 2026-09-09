using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ExtratoProcessador.Worker;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };

        using var connection = await factory.CreateConnectionAsync(stoppingToken);
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "extratos", durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);

        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, eventArgs) => 
        {
            var body = eventArgs.Body.ToArray();
            var conteudo = Encoding.UTF8.GetString(body);

            logger.LogInformation("Extrato recebido: {Conteudo}", conteudo);

            await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        };

        await channel.BasicConsumeAsync(queue: "extratos", autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
