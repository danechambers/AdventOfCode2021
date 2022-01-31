namespace DataCruncher;

public class Worker : BackgroundService
{
    private readonly IEnumerable<IDataCruncher> dataCrunchers;
    private readonly ILogger<Worker> _logger;
    private readonly IHostApplicationLifetime appLifetime;

    public Worker(
        IEnumerable<IDataCruncher> dataCrunchers,
        ILogger<Worker> logger,
        IHostApplicationLifetime appLifetime)
    {
        this.dataCrunchers = dataCrunchers;
        _logger = logger;
        this.appLifetime = appLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var crunchTasks =
            dataCrunchers
                .Select(cruncher => cruncher.Crunch(stoppingToken))
                .ToList();

        while (!stoppingToken.IsCancellationRequested && crunchTasks.Any())
        {
            var finishedTask = await Task.WhenAny(crunchTasks);
            crunchTasks.Remove(finishedTask);
            var result = await finishedTask;
            _logger.LogInformation(result.ToString());
        }

        appLifetime.StopApplication();
    }
}
