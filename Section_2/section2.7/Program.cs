﻿using StackExchange.Redis;

var muxer = ConnectionMultiplexer.Connect("localhost");
var db = muxer.GetDatabase();

// TODO for Coding Challenge Start here on starting-point branch

#region Prepared Script


var scriptText = @"
    local id = redis.call('incr', @id_key)
    local key = 'key:' .. id
    redis.call('set', key, @value)
    return key
";

LuaScript script = LuaScript.Prepare(scriptText);

RedisResult key1 = db.ScriptEvaluate(script, new { id_key = (RedisKey)"autoIncrement", value = "Trial value" });
RedisResult key2 = db.ScriptEvaluate(script, new { id_key = (RedisKey)"autoIncrement", value = "Another Trial value" });


Console.WriteLine($"Key 1: {key1}");
Console.WriteLine($"Key 2: {key2}");
#endregion

#region Non-Prepared Script

string nonPreparedScript = @"
    local id = redis.call('incr', KEYS[1])
    local key =  'key:'  .. id
    redis.call('set', key, ARGV[1])
    return key
";
var key3 = db.ScriptEvaluate(nonPreparedScript, new RedisKey[] { "autoIncrement" }, new RedisValue[] { "Okay Another Trial" });

Console.WriteLine($"Key 3: {key3}");
#endregion
//end coding challenge
