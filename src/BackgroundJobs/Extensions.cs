using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SGSX.Extensions.BackgroundJobs.Multiplexers;

namespace SGSX.Extensions.BackgroundJobs;
public static class Extensions
{
    public static IServiceCollection AddBackgroundJobs(this IServiceCollection services, MultiplexerType multiplexerType, uint concurrency = 3)
    {
        services.AddMultiplexer(multiplexerType, concurrency);

        services.AddWorkers(concurrency);

        return services;
    }

    public static IServiceCollection AddBackgroundJobs<TMultiplexer>(this IServiceCollection services, Func<IServiceProvider, TMultiplexer> factory, uint concurrency = 3) where TMultiplexer : Multiplexer
    {
        services.AddSingleton<Multiplexer, TMultiplexer>(factory);

        services.AddWorkers(concurrency);

        return services;
    }

    private static void AddWorkers(this IServiceCollection services, uint concurrency)
    {
        for (int i = 0; i < concurrency; i++)
        {
            services.AddSingleton<IHostedService>(BackgroundWorker.Create);
        }
    }

    private static void AddMultiplexer(this IServiceCollection services, MultiplexerType type, uint concurrency)
    {
        switch (type)
        {
            case MultiplexerType.Undefined:
                throw new ArgumentException("undefined type", nameof(type));

            case MultiplexerType.Fanout:
                {
                    services.AddSingleton<Multiplexer, FanOutMultiplexer>(sp => new FanOutMultiplexer(concurrency));
                    break;
                }
            case MultiplexerType.RandomSpread:
                {
                    services.AddSingleton<Multiplexer, RandomSpreadMultiplexer>(sp => new RandomSpreadMultiplexer(concurrency));
                    break;
                }

            default:
                {
                    services.AddSingleton<Multiplexer, RandomSpreadMultiplexer>(sp => new RandomSpreadMultiplexer(concurrency));
                    break;
                }
        }

        services.AddSingleton<IWorkPublisher>(sp => sp.GetRequiredService<Multiplexer>());
        services.AddSingleton<IWorkListener>(sp => sp.GetRequiredService<Multiplexer>());
    }
}
