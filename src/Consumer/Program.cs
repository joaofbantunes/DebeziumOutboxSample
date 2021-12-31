using Consumer;
using Events;
using Events.Kafka;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton(new KafkaOrderEventConsumerSettings("consumer"));
        services.AddSingleton<IEventConsumer, KafkaEventConsumer>();
        services.AddSingleton(typeof(JsonEventSerializer<>));
        services.AddHostedService<Worker>();
        services.AddLogging(loggingBuilder => loggingBuilder.AddSeq(serverUrl: "http://seq:5341"));
    })
    .Build();

await host.RunAsync();