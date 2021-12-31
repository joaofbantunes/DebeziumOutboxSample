namespace Events;

public abstract record EventBase(Guid Id, DateTime OccurredAt)
{
    public abstract string Type { get; }
}