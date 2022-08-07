using grpc;

namespace HomeAssistant.API.Workers;

public class Worker : BackgroundService
{
    private readonly Greeter.GreeterClient _client;
    public Worker(Greeter.GreeterClient client)
    {
        _client = client;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //throw new NotImplementedException();
        var count = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            count++;

            var reply = await _client.SayHelloAsync(
                new HelloRequest { Name = $"Worker {count}" });

            //_greetRepository.SaveGreeting(reply.Message);
            Console.WriteLine(reply.Message);

            await Task.Delay(1000, stoppingToken);
        }
    }
}