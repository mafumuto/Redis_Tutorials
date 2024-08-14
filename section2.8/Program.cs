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

var success1 = transaction.Execute();
Console.WriteLine($"Transaction Successful - 1: {success1}");

var person1Entry = db.HashGetAll("person:1");
transaction.AddCondition(Condition.HashEqual("person:1", "age", 32));
transaction.HashIncrementAsync("person:1", "age");
transaction.SortedSetIncrementAsync("person:age", "person:1", 1);


var success2 = transaction.Execute();
Console.WriteLine($"Transaction Successful - 2: {success2}");

var ageSet = db.SortedSetScan("person:age");

Console.WriteLine($"Person:1 field: \n+ {string.Join("\n+ ", person1Entry)}\n");
Console.WriteLine($"Age Set field: \n- {string.Join("\n- ", ageSet)}\n");


// end coding challenge
