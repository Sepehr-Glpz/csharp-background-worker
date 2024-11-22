
namespace SGSX.Extensions.BackgroundJobs.Multiplexers;
internal class FanOutMultiplexer(uint concurrency) : Multiplexer(concurrency)
{
    public override async ValueTask PublishAsync<TWork>(TWork work, CancellationToken ct = default)
    {
        var tasks = new List<Task>();

        foreach (var (chan, _) in ReadChannels.Where(c => c.Open))
        {
            tasks.Add(chan.Writer.WriteAsync(work, ct).AsTask());
        }

        await Task.WhenAll(tasks);
    }
}