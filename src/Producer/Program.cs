using Microsoft.EntityFrameworkCore;
using Producer;
using Producer.Data;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<DemoSetupHostedService>();
        services.AddHostedService<Worker>();
        services.AddDbContext<SampleDbContext>(
            options => options.UseNpgsql(
                "server=postgres;port=5432;user id=user;password=pass;database=DebeziumOutboxSample"));
        services.AddLogging(loggingBuilder => loggingBuilder.AddSeq(serverUrl: "http://seq:5341"));
    })
    .Build();

await host.RunAsync();