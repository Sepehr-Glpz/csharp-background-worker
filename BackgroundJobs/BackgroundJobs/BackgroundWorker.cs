using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SGSX.Extensions.BackgroundJobs;
internal class BackgroundWorker(IWorkListener jobs) : BackgroundService
{
    public static BackgroundWorker Create(IServiceProvider sp) => new(sp.GetRequiredService<IWorkListener>());

    private readonly IWorkListener _jobs = jobs;

    private Guid Id { get; } = Guid.NewGuid();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await foreach (var job in _jobs.ListenAsync(stoppingToken))
            {
                try
                {
                    var meta = new ExecutionMeta(Id);

                    await job.ExecuteAsync(meta, stoppingToken);
                }
                catch
                {
                    continue;
                }
            }
        }
        catch (OperationCanceledException)
        {
            return;
        }
    }
}
