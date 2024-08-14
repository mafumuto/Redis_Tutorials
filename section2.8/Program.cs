using StackExchange.Redis;

var muxer = ConnectionMultiplexer.Connect("localhost");
var db = muxer.GetDatabase();

// TODO for Coding Challenge Start here on starting-point branch
ITransaction transaction = db.CreateTransaction();

transaction.HashSetAsync("person:1", new HashEntry[]
{
    new HashEntry("name", "Steve"),
    new HashEntry("age", 32),
    new HashEntry("postal_code", "32999")
});
transaction.SortedSetAddAsync("person:name:Steve", "person:1", 0);
transaction.SortedSetAddAsync("person:postal_code:32999", "person:1", 0);
transaction.SortedSetAddAsync("person:age", "person:1", 32);

var success = transaction.Execute();
Console.WriteLine($"Transaction Successful: {success}");


// end coding challenge
