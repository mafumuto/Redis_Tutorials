using StackExchange.Redis;

var muxer = ConnectionMultiplexer.Connect("localhost");
var db = muxer.GetDatabase();

// TODO for Coding Challenge Start here on starting-point branch
ISubscriber subscriber = muxer.GetSubscriber();
var cancellationTokenSource = new CancellationTokenSource();
CancellationToken token = cancellationTokenSource.Token;

#region Simple Sequential
ChannelMessageQueue channel = await subscriber.SubscribeAsync("test-channel");

channel.OnMessage(msg =>
{
    Console.WriteLine($"Sequentially received {msg.Message} on channel {msg.Channel}");
});
#endregion

#region Simple Concurrent
await subscriber.SubscribeAsync("test-channel", (channel, value) =>
{
    Console.WriteLine($"Received: {value} on channel {channel}");
});
#endregion

#region Create Producer
var basicSendTask = Task.Run(async () =>
{
    var i = 0;
    while (!token.IsCancellationRequested)
    {
        await db.PublishAsync("test-channel", i++);
        await Task.Delay(1000);
    }
});
#endregion

#region Subscribe to Pattern
var patternSendTask = Task.Run(async () =>
{
    var i = 0;
    while (!token.IsCancellationRequested)
    {
        await db.PublishAsync($"pattern:{Guid.NewGuid()}", i++); //make the pattern pattern:*, then whenever you send any message to a channel matching that pattern
        await Task.Delay(1000);
    }
});

Console.ReadKey();
#endregion

#region Unsubscribe from Sequential Channel

Console.WriteLine("\nUnsubscribing to a single channel");
await channel.UnsubscribeAsync();
Console.ReadKey();

#endregion

#region Unsubscribe from Concurrent Subscriber

Console.WriteLine("\nUnsubscribing whole subscriber from test-channel");
await subscriber.UnsubscribeAsync("test-channel");
Console.ReadKey();

#endregion

#region Unsubscribe from Everything

Console.WriteLine("\nUnsubscribing from all");
await subscriber.UnsubscribeAllAsync();
Console.ReadKey();

#endregion
// end coding challenge
