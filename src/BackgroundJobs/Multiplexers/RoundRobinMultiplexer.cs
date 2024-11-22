namespace SGSX.Extensions.BackgroundJobs.Multiplexers;
internal class RoundRobinMultiplexer(uint concurrency) : Multiplexer(concurrency)
{
    private uint _current = 0;

    private readonly object _syncRoot = new();

    public override async ValueTask PublishAsync<TWork>(TWork work, CancellationToken ct = default)
    {
        var (chan, _) = this.ReadChannels[Next()];

        await chan.Writer.WriteAsync(work, ct);
    }

    private int Next()
    {
        lock (_syncRoot)
        {
            uint res = _current;

            if (_current + 1 > Concurrency)
            {
                _current = 0;
            }

            checked
            {
                return (int)res;
            }
        }
    }
}
