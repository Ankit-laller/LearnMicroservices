using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
namespace BoookingService.Utils
{
    public static class RabbitMqHelper
    {
        private static readonly string _hostName = "your host url";
        private static readonly string _queueName = "demo-queue";

        /// <summary>
        /// Asynchronously publish a message to the queue.
        /// </summary>


        //public static async Task SendMessageAsync<T>(T obj)
        //    {
        //            var factory = new ConnectionFactory();
        //            factory.Uri = new Uri(_hostName);
        //            await using var connection = await factory.CreateConnectionAsync();
        //            await using var channel = await connection.CreateChannelAsync();

        //            await channel.QueueDeclareAsync(
        //                queue: "demo-queue",
        //                durable: false,
        //                exclusive: false,
        //                autoDelete: false,
        //                arguments: null,
        //                cancellationToken: CancellationToken.None
        //            );

        //            // Serialize object to JSON
        //            var json = JsonSerializer.Serialize(obj);
        //            var body = Encoding.UTF8.GetBytes(json);

        //            var properties = new BasicProperties { Persistent = false };

        //            await channel.BasicPublishAsync(
        //                exchange: "",
        //                routingKey: "demo-queue",
        //                mandatory: false,
        //                basicProperties: properties,
        //                body: body.AsMemory(),
        //                cancellationToken: CancellationToken.None
        //            );
        //    }


        /// <summary>
        /// Asynchronously consume messages from the queue.
        /// </summary>
        //public static async Task ReceiveMessagesAsync(CancellationToken cancellationToken = default)
        //    {
        //        var factory = new ConnectionFactory();
        //        factory.Uri = new Uri(_hostName);
        //        var connection = await factory.CreateConnectionAsync();
        //        var channel = await connection.CreateChannelAsync();

        //        await channel.QueueDeclareAsync(
        //            queue: _queueName,
        //            durable: false,
        //            exclusive: false,
        //            autoDelete: false,
        //            arguments: null
        //        );

        //        var consumer = new AsyncEventingBasicConsumer(channel);

        //        consumer.ReceivedAsync += async (sender, ea) =>
        //        {
        //            var body = ea.Body.ToArray();
        //            var message = Encoding.UTF8.GetString(body);

        //            Console.WriteLine($" [x] Received: {message}");

        //            // simulate async work
        //            await Task.Yield();
        //        };


        //        await channel.BasicConsumeAsync(
        //            queue: _queueName,
        //            autoAck: true,
        //            consumer: consumer
        //        );

        //        Console.WriteLine(" [*] Waiting for messages. Press Ctrl+C to stop.");
        //        try
        //        {
        //            await Task.Delay(Timeout.Infinite, cancellationToken);
        //        }
        //        catch (TaskCanceledException)
        //        {
        //            Console.WriteLine(" [*] Consumer stopped.");
        //        }
        //    }



        public static async Task<TResponse> SendRequestAsync<TRequest, TResponse>(
        string requestQueue,
        TRequest request,
        TimeSpan timeout)
        {
            var factory = new ConnectionFactory { Uri = new Uri(_hostName) };
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            // Create a temporary reply queue
            var replyQueue = await channel.QueueDeclareAsync(queue: "", exclusive: true);
            var correlationId = Guid.NewGuid().ToString();

            var tcs = new TaskCompletionSource<TResponse>();

            // Consumer for the reply
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var response = JsonSerializer.Deserialize<TResponse>(json);
                    tcs.TrySetResult(response!);
                }
                await Task.Yield();
            };

            await channel.BasicConsumeAsync(replyQueue.QueueName, true, consumer);

            // Publish request
            var props = new BasicProperties
            {
                CorrelationId = correlationId,
                ReplyTo = replyQueue.QueueName
            };

            var jsonRequest = JsonSerializer.Serialize(request);
            var body = Encoding.UTF8.GetBytes(jsonRequest);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: requestQueue,
                mandatory: false,
                basicProperties: props,
                body: body);

            // Wait for reply or timeout
            using var cts = new CancellationTokenSource(timeout);
            await using (cts.Token.Register(() => tcs.TrySetCanceled()))
            {
                return await tcs.Task;
            }
        }

    }


}
