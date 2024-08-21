// See https://aka.ms/new-console-template for more information

using StackExchange.Redis;

var muxer = ConnectionMultiplexer.Connect("localhost");
var db = muxer.GetDatabase();

// TODO for Coding Challenge Start here on starting-point branch


//Command Flags provide some useful operational features, particularly around server selection and performance tuning.
//For example, if you are running a read-command, and do not want to hit your master shard, you can use the command flag CommandFlags.DemandReplica.
//There are corresponding flags to demand a master shard, prefer a master shard, or prefer a replica shard.

//Another really useful thing you can drive via the command flags is to set a command to fire-and-forget.
//If you set a command to fire and forget, StackExchange.Redis will read back the result of your command,
//and release execution as soon as it sends the command to Redis, this can increase throughput, particularly in cases where you do not care about the result of a command.

string storePath = "Data";
string textPath = "moby_dick.txt";
string tutorialTxtPath = Path.Combine(storePath, textPath);

char[] delimiterChars = { ' ', ',', '.', ':', '\t', '\n', '—', '?', '"', ';', '!', '’', '\r', '\'', '(', ')', '”' };

var t1 = Directory.GetCurrentDirectory();
var t2 = System.AppDomain.CurrentDomain.BaseDirectory;

Console.WriteLine($"T1; {t1}");
Console.WriteLine($"T2; {t2}");
Console.WriteLine($"TutorialTxtPath; {tutorialTxtPath}");

await db.KeyDeleteAsync("bf");
await db.KeyDeleteAsync("cms");
await db.KeyDeleteAsync("topk");

//string fullText;
try
{
    string fullText = await File.ReadAllTextAsync(tutorialTxtPath);

    Console.WriteLine($"Is text null: {string.IsNullOrEmpty(fullText)}");

    string[]? words = fullText.Split(delimiterChars).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.ToLower()).Select(x=>x).ToArray();

    HashSet<object> bloomList = words.Aggregate(new HashSet<object> { "bf" }, (list, word) =>
    {
        list.Add(word);
        return list;
    });

    Console.WriteLine($"words count: {words.Length}");
    Console.WriteLine($"bloomList Count: {bloomList.Count}");

    //var reserveAmount = Console.ReadLine();

    await db.ExecuteAsync("BF.RESERVE", "bf", 0.01, 20000);

    await db.ExecuteAsync("BF.MADD", bloomList, CommandFlags.FireAndForget);

    await db.ExecuteAsync("TOPK.RESERVE", "topk", 10, 20, 10, .925);

    List<object> topKList = words.Aggregate(new Dictionary<string, int>(), (dict, word) =>
    {
        if (!dict.ContainsKey(word))
        {
            dict.Add(word, 0);
        }
        dict[word]++;
        return dict;
    }).Aggregate(new List<object> { "topk" }, (list, dict) =>
    {
        list.Add(dict.Key);
        list.Add(dict.Value);
        return list;
    });

    await db.ExecuteAsync("TOPK.INCRBY", topKList, CommandFlags.FireAndForget); //en sık tekrar eden

    var doesTheExist = await db.ExecuteAsync("BF.EXISTS", "bf", "the");

    Console.WriteLine($"Typeof {nameof(doesTheExist)}: {doesTheExist.GetType()}; {doesTheExist}");

    var doesTheExistAsInt = (int)doesTheExist;
    Console.WriteLine($"Typeof {nameof(doesTheExistAsInt)}: {doesTheExistAsInt.GetType()}; {doesTheExistAsInt}");

    var doesTheExistAsDouble = (double)doesTheExist;
    Console.WriteLine($"Typeof {nameof(doesTheExistAsDouble)}: {doesTheExistAsDouble.GetType()}; {doesTheExistAsDouble}");

    var res = await db.ExecuteAsync("TOPK.LIST", "topk");
    var arr = ((RedisResult[])res!).Select(x => x.ToString());
    Console.WriteLine($"Top 10: {string.Join(", ", arr)}");

    var withCounts = (await db.ExecuteAsync("TOPK.LIST", "topk", "WITHCOUNT")).ToDictionary().Select(x => $"{x.Key}: {x.Value}");
    Console.WriteLine($"Top 10, with counts: {string.Join(", ", withCounts)}");
}
catch (Exception e)
{
    Console.WriteLine($"Error: {e.Message}");

}





/*
var arguments = new object[] { "foo", "bar" };
await db.ExecuteAsync("Set", arguments);
*/

// end coding challenge