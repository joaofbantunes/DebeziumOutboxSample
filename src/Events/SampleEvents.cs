namespace Events;

public abstract record OrderEventBase(Guid Id, DateTime OccurredAt, Guid OrderId, Guid DishId, string CustomerNumber)
    : EventBase(Id, OccurredAt);

public record OrderCreated(Guid Id, DateTime OccurredAt, Guid OrderId, Guid DishId, string CustomerNumber)
    : OrderEventBase(Id, OccurredAt, OrderId, DishId, CustomerNumber)
{
    public override string Type => nameof(OrderCreated);
}

public record OrderDelivered(Guid Id, DateTime OccurredAt, Guid OrderId, Guid DishId, string CustomerNumber)
    : OrderEventBase(Id, OccurredAt, OrderId, DishId, CustomerNumber)
{
    public override string Type => nameof(OrderDelivered);
}

public record OrderCancelled(Guid Id, DateTime OccurredAt, Guid OrderId, Guid DishId, string CustomerNumber, string? Reason)
    : OrderEventBase(Id, OccurredAt, OrderId, DishId, CustomerNumber)
{
    public override string Type => nameof(OrderCancelled);
}
