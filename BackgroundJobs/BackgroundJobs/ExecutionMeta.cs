
namespace SGSX.Extensions.BackgroundJobs;
public readonly struct ExecutionMeta
{
    private readonly Dictionary<string, object> _meta;

    public ExecutionMeta(Guid executorId)
    {
        _meta = new()
        {
            [nameof(ExecutionDate)] = DateTime.UtcNow,
            [nameof(ExecutionId)] = Guid.NewGuid(),
            [nameof(ExecutorId)] = executorId,
        };
    }

    public TData? Get<TData>(string key)
    {
        if (_meta.TryGetValue(key, out var value) && value is TData data)
        {
            return data;
        }

        return default;
    }

    public Guid ExecutorId => Get<Guid>(nameof(ExecutorId));
    public Guid ExecutionId => Get<Guid>(nameof(ExecutionId));
    public DateTime ExecutionDate => Get<DateTime>(nameof(ExecutionDate));
}