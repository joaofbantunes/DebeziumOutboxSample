using System.Text;
using System.Text.Json;
using Confluent.Kafka;

namespace Events.Kafka;

public class JsonEventSerializer<T> : ISerializer<T>, IDeserializer<T> where T : class
{
    private readonly Dictionary<string, Type> _eventTypeMap;

    public JsonEventSerializer()
    {
        _eventTypeMap = typeof(T)
            .Assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(T)))
            .ToDictionary(t => t.Name, t => t);
    }

    public byte[] Serialize(T data, SerializationContext context)
        => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
        {
            // 🤷‍♂️library didn't really take nulls into account
            return null!; 
            // in any case, shouldn't happen, we're not supposed to have empty events
        }

        var eventType = Encoding.UTF8.GetString(context.Headers.GetLastBytes("eventType"));
        var deserialized = (T) JsonSerializer.Deserialize(data, _eventTypeMap[eventType])!;
        return deserialized;
    }
}