using StackExchange.Redis;
var muxer = ConnectionMultiplexer.Connect("localhost");
var db = muxer.GetDatabase();

// TODO for Coding Challenge Start here on starting-point branch

var allUsersSet = "users";
var activeUsersSet = "users:state:active";
var inactiveUsersSet = "users:state:inactive";
var offlineUsersSet = "users:state:offline";
db.KeyDelete(new RedisKey[] { allUsersSet, activeUsersSet, inactiveUsersSet, offlineUsersSet });

int index = 0;
RedisValue[] CreateDefaultUserArray(int count)
{
    RedisValue[] userArray = new RedisValue[count];
    for (int i = 0; i < count; i++)
    {
        index++;
        userArray[i] = new RedisValue($"User:{index}");

    }

    return userArray;
}

db.SetAdd(activeUsersSet, CreateDefaultUserArray(10));
db.SetAdd(inactiveUsersSet, CreateDefaultUserArray(3));
db.SetAdd(offlineUsersSet, CreateDefaultUserArray(7));

long usersLenght = db.SetCombineAndStore(SetOperation.Union,
                                        allUsersSet,
                                        new RedisKey[] { activeUsersSet, inactiveUsersSet, offlineUsersSet });

Console.WriteLine($"Users count: {usersLenght}");
Console.WriteLine($"All users list: \n#{string.Join(",\n# ", db.SetMembers(allUsersSet))}\n"); //SMEMBERS

RedisValue user6 = "User:6";
RedisValue user17 = "User:17";
Console.WriteLine($"Is {user6} offline? {db.SetContains(offlineUsersSet, user6)}");
Console.WriteLine($"Is {user17} offline? {db.SetContains(offlineUsersSet, user17)}");

Console.WriteLine($"All online users with scan: \n+ {string.Join(",\n+ ", db.SetScan(activeUsersSet))}\n"); //SSCAN

Console.WriteLine("Moving User:1 from active to offline");
var moved = db.SetMove(activeUsersSet, offlineUsersSet, "User:1");
Console.WriteLine($"Move Successful: {moved}\n");

Console.WriteLine($"All online users with scan: \n+ {string.Join(",\n+ ", db.SetScan(activeUsersSet))}\n");

// end coding challenge