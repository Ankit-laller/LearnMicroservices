using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
namespace CreditService.Utils
{
    public static class RabbitMqHelper
    {
        private static readonly string _hostName = "your host url";
        private static readonly string _queueName = "demo-queue";

        /// <summary>
        /// Publish any object as JSON.
        /// </summary>
        public static async Task SendMessageAsync<T>(T obj, string queueName)
        {
            var factory = new ConnectionFactory { Uri = new Uri(_hostName) };
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            var queue = queueName ?? _queueName;

            await channel.QueueDeclareAsync(queueName, durable: false, exclusive: false, autoDelete: false);

            var json = JsonSerializer.Serialize(obj);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties { Persistent = true };

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: queue,
                mandatory: false,
                basicProperties: properties,
                body: body.AsMemory()
            );
        }

        /// <summary>
        /// Subscribe to a queue with a callback.
        /// </summary>
        public static async Task ReceiveMessagesAsync(
            Func<string, Task> onMessageReceived,
            string? queueName = null,
            CancellationToken cancellationToken = default)
        {
            var factory = new ConnectionFactory { Uri = new Uri(_hostName) };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            var queue = queueName ?? _queueName;

            await channel.QueueDeclareAsync(queue, durable: false, exclusive: false, autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                await onMessageReceived(message); // delegate actual processing
            };

            await channel.BasicConsumeAsync(queue, autoAck: true, consumer: consumer);

            try
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine(" [*] Consumer stopped.");
            }
        }

        public static async Task StartConsumerAsync<TRequest, TResponse>(
    string queueName,
    Func<TRequest, Task<TResponse>> handler,
    CancellationToken cancellationToken = default)
        {
            var factory = new ConnectionFactory { Uri = new Uri(_hostName) };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var requestJson = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var request = JsonSerializer.Deserialize<TRequest>(requestJson);

                    var response = await handler(request!);

                    // Serialize response
                    var responseJson = JsonSerializer.Serialize(response);
                    var responseBody = Encoding.UTF8.GetBytes(responseJson);

                    var props = new BasicProperties
                    {
                        CorrelationId = ea.BasicProperties.CorrelationId
                    };

                    await channel.BasicPublishAsync(
                        exchange: "",
                        routingKey: ea.BasicProperties.ReplyTo,
                        mandatory: false,
                        basicProperties: props,
                        body: responseBody);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" [!] Error processing message: {ex.Message}");
                }

                await Task.Yield();
            };

            await channel.BasicConsumeAsync(queueName, true, consumer);

            Console.WriteLine($" [*] Listening on queue: {queueName}");

            try
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine(" [*] Consumer stopped.");
            }
        }


    }



}
