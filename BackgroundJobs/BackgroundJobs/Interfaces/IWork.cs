
namespace SGSX.Extensions.BackgroundJobs;
public interface IWork
{
    public ValueTask ExecuteAsync(ExecutionMeta meta, CancellationToken ct = default);
}
