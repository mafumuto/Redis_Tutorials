// See https://aka.ms/new-console-template for more information

using StackExchange.Redis;

Console.WriteLine("Hello, World!");
var muxer = ConnectionMultiplexer.Connect("localhost");
var db = muxer.GetDatabase();

// TODO for Coding Challenge Start here on starting-point branch

var userAgeSet = "users:age";
var userLastAccessSet = "users:lastAccess";
var userHighScoreSet = "users:highScores";
var namesSet = "names";
var mostRecentlyActive = "users:mostRecentlyActive";

db.KeyDelete(new RedisKey[] { userAgeSet, userLastAccessSet, userHighScoreSet, namesSet, mostRecentlyActive });


db.SortedSetAdd(userAgeSet,
     new SortedSetEntry[]
    {
            new("User:1", 20),
            new("User:2", 23),
            new("User:3", 18),
            new("User:4", 35),
            new("User:5", 55),
            new("User:6", 62)
    });

db.SortedSetAdd(userLastAccessSet,
    new SortedSetEntry[]
    {
            new("User:1", 1648483867),
            new("User:2", 1658074397),
            new("User:3", 1659132660),
            new("User:4", 1652082765),
            new("User:5", 1658087415),
            new("User:6", 1656530099)
    });

db.SortedSetAdd(userHighScoreSet,
    new SortedSetEntry[]
    {
            new("User:1", 10),
            new("User:2", 55),
            new("User:3", 36),
            new("User:4", 25),
            new("User:5", 21),
            new("User:6", 44)
    });

db.SortedSetAdd(namesSet,
    new SortedSetEntry[]
    {
            new("John", 0),
            new("Fred", 0),
            new("Bob", 0),
            new("Susan", 0),
            new("Alice", 0),
            new("Tom", 0)
    });

string user4 = "User:4";
string user3 = "User:3";

double? user4Score = db.SortedSetScore(userHighScoreSet, user4);
long? user4Rank = db.SortedSetRank(userHighScoreSet, user4, Order.Descending);
double? user3Score = db.SortedSetScore(userHighScoreSet, user3);
long? user3Rank = db.SortedSetRank(userHighScoreSet, user3, Order.Descending);
string alphabetizedStartPoint = "A";
string alphabetizedEndPoint = "K";

Console.WriteLine($"{user4} high score: {user4Score}");
Console.WriteLine($"{user4} is rank: {user4Rank}");
Console.WriteLine($"{user3} high score: {user3Score}");
Console.WriteLine($"{user3} is rank: {user3Rank}\n");

RedisValue[] highest3Score = db.SortedSetRangeByRank(userHighScoreSet, 0, 2, Order.Descending);
SortedSetEntry[] highest3ScoreWithScore = db.SortedSetRangeByRankWithScores(userHighScoreSet,0,2,Order.Descending);
RedisValue[]? namesAlphabetized = db.SortedSetRangeByValue(namesSet);
RedisValue[]? nameAlphabetizedBetweenAandJ = db.SortedSetRangeByValue(namesSet, 
                                                                    alphabetizedStartPoint, 
                                                                    alphabetizedEndPoint, 
                                                                    Exclude.Stop); // son nokta, durak noktası, dahil değil
db.SortedSetRangeAndStore(userLastAccessSet, mostRecentlyActive,0, 2); // sırala ve sakla

// double ifadesi userHighScoreSet kümesine 1 ağırlığı veriyor yani öncelik skor kümesinde. Normalde artan sırada getirir ama
// Reserve ile ters çevirerek azalan sıra haline getiriliyor 
var rankOrderMostRecentlyActive = db.SortedSetCombineWithScores(SetOperation.Intersect, 
                                    new RedisKey[] { userHighScoreSet, mostRecentlyActive }, 
                                    new double[] { 1, 0 }).Reverse(); 


Console.WriteLine($"Top three: \n# {string.Join("\n# ", highest3Score)}\n");
Console.WriteLine($"Top three with score: \n# {string.Join("\n# ", highest3ScoreWithScore)}\n");
Console.WriteLine($"Names Alphabetized: \n* {string.Join("\n* ", namesAlphabetized)}\n");
Console.WriteLine($"Names Alphabetized between A and J: \n> {string.Join("\n> ", nameAlphabetizedBetweenAandJ)}\n");


Console.WriteLine($"Highest Scores Most Recently Active: \n# {string.Join(",\n# ", rankOrderMostRecentlyActive)} \n");


// end coding challenge