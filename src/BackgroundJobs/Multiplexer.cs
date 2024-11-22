using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace SGSX.Extensions.BackgroundJobs;
public abstract class Multiplexer(uint concurrency) : IWorkPublisher, IWorkListener
{
    private readonly SemaphoreSlim _lock = new(1);

    public uint Concurrency { get; init; } = concurrency;

    protected IReadOnlyList<ChannelCell> ReadChannels { get; } = InitChannels(concurrency).ToList();

    public abstract ValueTask PublishAsync<TWork>(TWork work, CancellationToken ct = default) where TWork : IWork;

    public ValueTask PublishAsync(Func<ExecutionMeta, CancellationToken, ValueTask> work, CancellationToken ct = default) =>
        this.PublishAsync(new FuncWork(work), ct);

    /// <summary>
    /// Calling this method will open and block an internal channel
    /// </summary>
    /// <returns>An async enumerable of IWork to block on</returns>
    public virtual async IAsyncEnumerable<IWork> ListenAsync([EnumeratorCancellation] CancellationToken ct = default)
    {
        ChannelCell current;

        try
        {
            await _lock.WaitAsync(ct);

            current = ReadChannels.First(c => c.Open == false);

            current.OpenCell();
        }
        finally
        {
            _lock.Release();
        }

        try
        {
            await foreach (var item in current.Channel.Reader.ReadAllAsync(ct))
            {
                yield return item;
            }
        }
        finally
        {
            current.CloseCell();
        }
    }

    private static IEnumerable<ChannelCell> InitChannels(uint concurrency)
    {
        for (int index = 0; index < concurrency; index++)
        {
            yield return new ChannelCell(Channel.CreateUnbounded<IWork>());
        }
    }

    protected class ChannelCell(Channel<IWork> channel)
    {
        public Channel<IWork> Channel { get; } = channel;

        public void OpenCell() => _open = true;
        public void CloseCell() => _open = false;

        private volatile bool _open = false;
        public bool Open => _open;

        public void Deconstruct(out Channel<IWork> chan, out bool open) => (chan, open) = (Channel, Open);
    }
}
