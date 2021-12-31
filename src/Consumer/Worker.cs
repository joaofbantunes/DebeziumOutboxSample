using Events;

namespace Consumer;

public class Worker : BackgroundService
{
    private readonly IEventConsumer _eventConsumer;
    private readonly ILogger<Worker> _logger;

    public Worker(IEventConsumer eventConsumer, ILogger<Worker> logger)
    {
        _eventConsumer = eventConsumer;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => _eventConsumer.Subscribe(ConsumeEvent, stoppingToken);

    private void ConsumeEvent(OrderEventBase @event)
        => _logger.LogInformation("Event of type {eventType} received by the consumer application", @event.Type);
}