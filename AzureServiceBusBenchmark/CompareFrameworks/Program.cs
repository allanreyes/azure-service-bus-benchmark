using Azure.Messaging.ServiceBus;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Shared;
using System.Threading.Tasks;

public class Program
{
    public static void Main(string[] args)
    {
        // dotnet run --configuration Release --framework net60 --filter * --join
        BenchmarkSwitcher.FromAssemblies(new[] { typeof(ServiceBusBenchmarks).Assembly }).Run(args);
    }

}

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.NetCoreApp31)]
public class ServiceBusBenchmarks
{
    private readonly ServiceBusHelper helper = new ServiceBusHelper();
    private readonly ServiceBusSender _sender;
    private readonly string _payload = new string('a', 10240);

    public ServiceBusBenchmarks()
    {
        _sender = helper.GetTopicSender();
    }

    [Benchmark(Baseline = true)]
    public async Task UseSingle()
    {
        await _sender.SendMessageAsync(new ServiceBusMessage(_payload));
    }
}