namespace SGSX.Extensions.BackgroundJobs;
public interface IWorkPublisher
{
    ValueTask PublishAsync<TWork>(TWork work, CancellationToken ct = default) where TWork : IWork;
    ValueTask PublishAsync(Func<ExecutionMeta, CancellationToken, ValueTask> work, CancellationToken ct = default);
}
