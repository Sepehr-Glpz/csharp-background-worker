namespace SGSX.Extensions.BackgroundJobs.Multiplexers;

internal class RandomSpreadMultiplexer(uint concurrency) : Multiplexer(concurrency)
{
    private readonly Random _rng = new();

    public override async ValueTask PublishAsync<TWork>(TWork work, CancellationToken ct = default)
    {
        var channels = ReadChannels.Where(c => c.Open).ToArray();

        var target = channels[_rng.Next(channels.Length)];

        await target.Channel.Writer.WriteAsync(work, ct);
    }
}