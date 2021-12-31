using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Events.Kafka;

public class KafkaEventConsumer : IEventConsumer, IDisposable
{
    private readonly ILogger<KafkaEventConsumer> _logger;
    private readonly IConsumer<string, OrderEventBase> _consumer;

    public KafkaEventConsumer(
        ILogger<KafkaEventConsumer> logger,
        JsonEventSerializer<OrderEventBase> serializer,
        KafkaOrderEventConsumerSettings settings)
    {
        _logger = logger;

        var conf = new ConsumerConfig
        {
            GroupId = settings.ConsumerGroup,
            BootstrapServers = "broker:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<string, OrderEventBase>(conf)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(serializer)
            .Build();
    }

    public Task Subscribe(Action<OrderEventBase> callback, CancellationToken ct)
    {
        _logger.LogInformation("Subscribing");
        _consumer.Subscribe("order.events");

        var tcs = new TaskCompletionSource<bool>();

        // polling for messages is a blocking operation,
        // so spawning a new thread to keep doing it in the background
        var thread = new Thread(() =>
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Waiting for message...");

                    var message = _consumer.Consume(ct);

                    _logger.LogInformation(
                        "Received event {eventId}, of type {eventType}!\n{event}",
                        message.Message.Value.Id,
                        message.Message.Value.GetType().Name,
                        message.Message.Value);

                    callback(message.Message.Value);

                    _consumer.Commit(); // note: committing every time can have a negative impact on performance
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    _logger.LogInformation("Shutting down gracefully.");
                }
                catch (Exception ex)
                {
                    // TODO: implement error handling/retry logic
                    // like this, the failed message will eventually be "marked as processed"
                    // (commit to a newer offset) even though it failed
                    _logger.LogError(ex, "Error occurred when consuming event!");
                }
            }

            tcs.SetResult(true);
        })
        {
            IsBackground = true
        };

        thread.Start();

        return tcs.Task;
    }

    public void Dispose()
    {
        try
        {
            _consumer?.Close();
        }
        catch (Exception)
        {
            // no exceptions in Dispose :)
        }

        _consumer?.Dispose();
    }
}