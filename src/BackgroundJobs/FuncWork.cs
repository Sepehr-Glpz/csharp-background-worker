
namespace SGSX.Extensions.BackgroundJobs;
internal sealed class FuncWork(Func<ExecutionMeta, CancellationToken, ValueTask> work) : IWork
{
    public ValueTask ExecuteAsync(ExecutionMeta meta, CancellationToken ct = default) => work(meta, ct);
}