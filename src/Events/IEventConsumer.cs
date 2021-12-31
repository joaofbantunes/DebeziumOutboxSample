namespace Events;

public interface IEventConsumer
{
    Task Subscribe(Action<OrderEventBase> callback, CancellationToken ct);
}