using System.Net.Mime;
using System.Text;
using Polly;
using Producer.Data;

namespace Producer;

public class DemoSetupHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DemoSetupHostedService> _logger;

    public DemoSetupHostedService(IServiceScopeFactory scopeFactory, ILogger<DemoSetupHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await InitializeDatabaseAsync();

        await InitializeDebeziumAsync();

        async Task InitializeDatabaseAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope
                .ServiceProvider
                .GetRequiredService<SampleDbContext>();

            await db
                .Database
                .EnsureCreatedAsync(cancellationToken);
        }

        async Task InitializeDebeziumAsync()
        {
            var policy = Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            
            using var httpClient = new HttpClient();

            await policy.ExecuteAsync(
                async () =>
                {
                    var response = await httpClient.PutAsync(
                        "http://connect:8083/connectors/outbox-connector/config",
                        new StringContent(
                            File.ReadAllText("debezium-outbox-config.json"),
                            Encoding.UTF8,
                            MediaTypeNames.Application.Json)
                    );

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation(
                            "Debezium outbox configured. Status code: {statusCode} Response: {response}",
                            response.StatusCode, await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        _logger.LogError("Failed to configure Debezium outbox. Status code: {statusCode}",
                            response.StatusCode);
                        throw new Exception("Failed to configure Debezium outbox.");
                    }
                });
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // no-op
        return Task.CompletedTask;
    }
}