using StackExchange.Redis;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Connect to Redis
        // ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");

        var options = new ConfigurationOptions
        {
            EndPoints = { "localhost:6381" },
            Proxy = Proxy.Twemproxy
        };

        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(options);


        // Create 100 threads
        Task[] tasks = new Task[1];
        for (int i = 0; i < 1; i++)
        {
            tasks[i] = Task.Run(async () =>
            {
                for (int j = 0;; j++)
                {
                    // Get a reference to the database
                    IDatabase db = redis.GetDatabase();

                    // Set a key-value pair
                    string key = $"key-{i}-{j}";
                    string value = $"value-{i}-{j}";
                    db.StringSet(key, value);

                    // Read the value of the key
                    string readValue = db.StringGet(key);
                    Console.WriteLine($"Thread {i}, Connection {j}: READ {key}={readValue}");

                    // Increment the value of a key
                    db.StringIncrement($"{key}-counter");

                    // Get the value of the counter
                    var counter = db.StringGet($"{key}-counter");
                    Console.WriteLine($"Thread {i}, Connection {j}: INCR {key}-counter={counter}");

                    // Add an item to a sorted set
                    db.SortedSetAdd($"{key}-scores", "Alice", 10);
                    db.SortedSetAdd($"{key}-scores", "Bob", 20);
                    db.SortedSetAdd($"{key}-scores", "Charlie", 30);

                    // Get the rank of an item in the sorted set
                    var rank = db.SortedSetRank($"{key}-scores", "Charlie");
                    Console.WriteLine($"Thread {i}, Connection {j}: ZRANK {key}-scores Charlie={rank}");

                    await Task.Delay(100);
                }
            });
        }

        // Wait for all threads to complete
        await Task.WhenAll(tasks);

        Console.WriteLine("All threads completed.");
    }
}
