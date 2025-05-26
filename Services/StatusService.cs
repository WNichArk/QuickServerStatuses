
public class StatusService
{
    public Dictionary<string, string> GetAll()
    {
        return new Dictionary<string, string>
        {
            { "Service1", "Running" },
            { "Service2", "Stopped" },
            { "Service3", "Running" }
        };
    }

    public void Set(string key, string value)
    {
        // Here you would typically update the status in a database or in-memory store.
        // For this example, we will just print to console.
        Console.WriteLine($"Setting {key} to {value}");
    }
}