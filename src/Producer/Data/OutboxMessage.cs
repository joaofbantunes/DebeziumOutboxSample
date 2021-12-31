using Events;

namespace Producer.Data;

public class OutboxMessage
{
    public OutboxMessage(EventBase payload, string aggregateId, string aggregateType)
    {
        Id = payload.Id;
        Payload = payload;
        AggregateId = aggregateId;
        AggregateType = aggregateType;
        Timestamp = DateTime.SpecifyKind(payload.OccurredAt, DateTimeKind.Unspecified);
        Type = payload.Type;
    }

    public Guid Id { get; }

    public EventBase Payload { get; }
    
    public string AggregateId { get; }
    
    public string AggregateType { get; }
    
    public string Type { get; }
    
    public DateTime Timestamp { get; }
}