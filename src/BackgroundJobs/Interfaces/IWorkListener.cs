
namespace SGSX.Extensions.BackgroundJobs;
public interface IWorkListener
{
    IAsyncEnumerable<IWork> ListenAsync(CancellationToken ct = default);
}
