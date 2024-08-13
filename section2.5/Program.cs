using StackExchange.Redis;

var muxer = ConnectionMultiplexer.Connect("localhost");
var db = muxer.GetDatabase();

// TODO for Coding Challenge Start here on starting-point branch

var person1 = "Person:1";
var person2 = "Person:2";
var person3 = "Person:3";

db.KeyDelete(new RedisKey[] { person1, person2, person3 });

db.HashSet(person1, new HashEntry[]
{
        new("name","Alice"),
        new("age", 33),
        new("email","alice@example.com")
});

db.HashSet(person2, new HashEntry[]
{
    new("name", "John"),
    new("age", 41),
    new("email", "john@example.com")
});

db.HashSet(person3, new HashEntry[]
{
    new("name", "Jessica"),
    new("age", 25),
    new("email", "jes@example.com")
});

HashEntry[] person2Entry = db.HashGetAll(person2);  // less than 1000 fields
IEnumerable<HashEntry> person3Entry = db.HashScan(person3); // very large hash with many thousands of fields

Console.WriteLine($"{person2}'s old age: {person2Entry[1]}\n");
Console.WriteLine($"{person2}'s fields: \n> {string.Join("\n> ", person2Entry)}\n");
Console.WriteLine($"{person3}'s fields: \n> {string.Join("\n> ", person3Entry)}\n");

long newAge = db.HashIncrement(person2, person2Entry[1].Name, 2);
RedisValue person1Name = db.HashGet(person1, "name");


Console.WriteLine($"{person2} is {newAge} years old\n");
Console.WriteLine($"{person1}'s name is {person1Name}\n");


// end coding challenge

