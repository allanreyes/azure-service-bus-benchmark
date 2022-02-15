using Azure.Messaging.ServiceBus;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Shared;

BenchmarkRunner.Run<ServiceBusBenchmarks>();

[MemoryDiagnoser]
public class ServiceBusBenchmarks
{
    private List<ServiceBusReceivedMessage> _messages;
    private readonly ServiceBusSender _sender;

    public ServiceBusBenchmarks()
    {
        var helper = new ServiceBusHelper();
        _sender = helper.GetTopicSender();
        _messages = new List<ServiceBusReceivedMessage>();
        var count = 100;
        for (int i = 0; i < count; i++)
            _messages.Add(ServiceBusModelFactory.ServiceBusReceivedMessage(BinaryData.FromString(new string('a', 10240))));
    }

    [Benchmark(Baseline = true)]
    public async Task UseEach()
    {
        foreach (var message in _messages)
            await _sender.SendMessageAsync(new ServiceBusMessage(message));
    }

    [Benchmark]
    public async Task UseBatch()
    {
        if (!_messages.Any()) return;

        Console.WriteLine($"Sending {_messages.Count} messages");

        var batch = await _sender.CreateMessageBatchAsync();
        var index = 0;
        while (true)
        {
            var message = _messages.ElementAtOrDefault(index);
            if (message == null) break;

            if (batch.TryAddMessage(new ServiceBusMessage(message)))
            {
                index++;
                var isLastMessage = index == _messages.Count;

                if (!isLastMessage)
                    continue;
            }

            await _sender.SendMessagesAsync(batch); // Batch is full or end of messages
            Console.WriteLine($"Sent batch of {batch.Count} messages");
            batch = await _sender.CreateMessageBatchAsync(); // Reset batch            
        }
    }
}
