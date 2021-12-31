using Bogus;
using Events;
using Producer.Data;

namespace Producer;

public class Worker : BackgroundService
{
    private readonly Faker _faker;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<Worker> _logger;

    public Worker(IServiceScopeFactory scopeFactory, ILogger<Worker> logger)
    {
        _faker = new Faker();
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Producing events...");
            
            await using var scope = _scopeFactory.CreateAsyncScope();
            await using var db = scope.ServiceProvider.GetRequiredService<SampleDbContext>();
            
            await db.OutboxMessages.AddRangeAsync(
                GenerateSampleEvents(),
                stoppingToken);

            await db.SaveChangesAsync(stoppingToken);
            
            _logger.LogInformation("Sleeping for a bit");
            
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private IEnumerable<OutboxMessage> GenerateSampleEvents()
        => Enumerable
            .Range(0, 1000)
            .Select(i =>
            {
                if (i % 3 == 0)
                {
                    return new OutboxMessage(
                        new OrderDelivered(
                            _faker.Random.Guid(),
                            DateTime.UtcNow,
                            _faker.Random.Guid(),
                            _faker.Random.Guid(),
                            _faker.Random.ReplaceNumbers("#########")),
                        _faker.Random.Guid().ToString(),
                        "order");
                }

                if (i % 5 == 0)
                {
                    return new OutboxMessage(
                        new OrderCancelled(
                            _faker.Random.Guid(),
                            DateTime.UtcNow,
                            _faker.Random.Guid(),
                            _faker.Random.Guid(),
                            _faker.Random.ReplaceNumbers("#########"),
                            _faker.Rant.Review()),
                        _faker.Random.Guid().ToString(),
                        "order");
                }

                return new OutboxMessage(
                    new OrderCreated(
                        _faker.Random.Guid(),
                        DateTime.UtcNow,
                        _faker.Random.Guid(),
                        _faker.Random.Guid(),
                        _faker.Random.ReplaceNumbers("#########")),
                    _faker.Random.Guid().ToString(),
                    "order");
            });
}