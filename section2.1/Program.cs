using StackExchange.Redis;

var muxer = ConnectionMultiplexer.Connect("localhost");
var db = muxer.GetDatabase();
// TODO for Coding Challenge Start here on starting-point branch

RedisKey instructorNameKey = new RedisKey("instructor:1:name");
db.StringSet(instructorNameKey, "Ozgur");
RedisValue instructor1Name = db.StringGet(instructorNameKey);

Console.WriteLine($"Instructor 1's Name: {instructor1Name}");

db.StringAppend(instructorNameKey, " Adam");
instructor1Name = db.StringGet(instructorNameKey);

Console.WriteLine($"Instructor 1's Name: {instructor1Name}");

RedisKey tempKey = new RedisKey("temperature");
db.StringSet(tempKey, 38);
long tempValue = db.StringIncrement(tempKey, 4);
db.StringIncrement(tempKey);
db.StringIncrement(tempKey, -0.5f);
RedisValue newTemp = db.StringGet(tempKey);

Console.WriteLine($"TempValue: {tempValue} \nNew Temperature: {newTemp}");

RedisKey trialKey = new RedisKey("Trial");
db.StringSet(trialKey,"Deneme", expiry: TimeSpan.FromSeconds(3));
var trialValue = db.StringGet(trialKey);
Console.WriteLine($"Trial Value: {trialValue}");

Thread.Sleep(4000);

var trialValueExpire = db.StringGet(trialKey);

Console.WriteLine($"Trial Value(Expired): {trialValueExpire}");

var conditionalKey = "ConditionalKey";
var conditionalKeyText = "this has been set";
var wasSet = db.StringSet(conditionalKey, conditionalKeyText, when: When.NotExists);

Console.WriteLine($"Key set: {wasSet}");

wasSet = db.StringSet(conditionalKey, "this text doesn't matter since it won't be set", when: When.NotExists);
var conditionalValue = db.StringGet(conditionalKey);

Console.WriteLine($"Key set: {wasSet}");
Console.WriteLine($"Conditional Value: {conditionalValue}");

// end coding challenge