
namespace SGSX.Extensions.BackgroundJobs;
public enum MultiplexerType : byte
{
    Undefined = 0,
    Fanout = 1,
    RandomSpread = 2,
    RoundRobin = 3,
}
