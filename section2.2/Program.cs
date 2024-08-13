using StackExchange.Redis;

var muxer = ConnectionMultiplexer.Connect("localhost");
var db = muxer.GetDatabase();
// TODO for Coding Challenge Start here on starting-point branch

var fruitsKey = "fruits";
var vegetablesKey = "vagetables";

db.KeyDelete(new RedisKey[] { fruitsKey, vegetablesKey });

db.ListLeftPush(fruitsKey, new RedisValue[] { "Banana", "Mango", "Apple", "Pepper", "Kiwi", "Grape" });

Console.WriteLine($"First fruit in the list is: {db.ListGetByIndex(fruitsKey, 0)}");
Console.WriteLine($"Last fruit in the list is: {db.ListGetByIndex(fruitsKey, -1)}");
Console.WriteLine($"Fruit indexes 0 to -1: {string.Join(", ", db.ListRange(fruitsKey))}\n");

db.ListRightPush(vegetablesKey, new RedisValue[] { "Potato", "Carrot", "Asparagus", "Beet", "Garlic", "Tomato" });

Console.WriteLine($"First vegetable in the list is: {db.ListGetByIndex(vegetablesKey, 0)}");
Console.WriteLine($"Last vegetable in the list is: {db.ListGetByIndex(vegetablesKey, -1)}");
Console.WriteLine($"Vegetable indexes 0 to -1: {string.Join(", ", db.ListRange(vegetablesKey))}\n");

db.ListMove(vegetablesKey,fruitsKey, ListSide.Right, ListSide.Left);

db.ListLeftPush(fruitsKey, "Peach");
Console.WriteLine($"Fruit indexes 0 to -1: {string.Join(", ", db.ListRange(fruitsKey))}\n");

RedisValue leftmostValue = db.ListLeftPop(fruitsKey);
Console.WriteLine($"Leftmost Value: {leftmostValue}");
Console.WriteLine($"Fruit indexes 0 to -1: {string.Join(", ", db.ListRange(fruitsKey))}\n");

db.ListRightPush(fruitsKey, new RedisValue[] {"Pineapple", "Lemon"});
Console.WriteLine($"Fruit indexes 0 to -1: {string.Join(", ", db.ListRange(fruitsKey))}\n");

int valueCount = 2;
RedisValue[] rightmostValue = db.ListRightPop(fruitsKey, valueCount);
Console.WriteLine($"Rightmost {valueCount} Value: {string.Join(", ", rightmostValue)}");
Console.WriteLine($"Fruit indexes 0 to -1: {string.Join(", ", db.ListRange(fruitsKey))}\n");

Console.WriteLine($"Position of Mango: {db.ListPosition(fruitsKey, "Mango")}\n");

db.ListRightPush(fruitsKey, "Kiwi");
db.ListRightPush(fruitsKey, "Watermelon");
Console.WriteLine($"Fruit indexes 0 to -1: {string.Join(", ", db.ListRange(fruitsKey))}\n");

Console.WriteLine($"Position of Kiwi: {db.ListPosition(fruitsKey, "Kiwi")}");
Console.WriteLine($"Position of Second Kiwi: {db.ListPosition(fruitsKey, "Kiwi", rank: 2, 10)}");
Console.WriteLine($"Position of Second Kiwi(out of range): {db.ListPosition(fruitsKey, "Kiwi", rank: 2, 5)}\n");

Console.WriteLine($"There are {db.ListLength(fruitsKey)} fruits in our Fruit List\n");




// end coding challenge