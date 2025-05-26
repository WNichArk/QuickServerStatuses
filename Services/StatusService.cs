using System.Text.Json;
using System.Text.Json.Serialization;
using QuickServerStatuses.Models.Servers;

public class StatusService
{
    private readonly string _filePath = Path.Combine(AppContext.BaseDirectory, "Persist/servers.json");
    private readonly object _lock = new();
    private List<Server> _servers = new();

    public StatusService()
    {
        Load();
    }

    public List<Server> GetAll()
    {
        lock (_lock)
        {
            _servers.RemoveAll(s => s.LastUpdate < DateTime.Now.AddDays(-2) && s.Status == ServerStatus.Unknown);
            _servers.ForEach( s =>
            {
                if(s.LastUpdate < DateTime.Now.AddMinutes(-30))
                {
                    s.Status = ServerStatus.Unknown;
                    s.Message = "Status unknown due to inactivity.";
                }
            });
            return _servers.OrderBy(s => s.Name).ToList();
        }
    }

    public void Set(Server server)
    {
        lock (_lock)
        {
            var existingServer = _servers.FirstOrDefault(s => s.Name == server.Name);
            if (existingServer != null)
            {
                existingServer.Status = server.Status;
                existingServer.Description = server.Description;
                existingServer.Message = server.Message;
                existingServer.LastUpdate = DateTime.Now;
            }
            else
            {
                server.LastUpdate = DateTime.Now;
                _servers.Add(server);
            }

            Save();
        }
    }

    private void Load()
    {
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            var loaded = JsonSerializer.Deserialize<List<Server>>(json, JsonOptions());
            if (loaded is not null)
            {
                _servers = loaded;
            }
        }
    }

    private void Save()
    {
        var json = JsonSerializer.Serialize(_servers, JsonOptions(true));
        File.WriteAllText(_filePath, json);
    }

    private static JsonSerializerOptions JsonOptions(bool writeIndented = false) =>
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() },
            WriteIndented = writeIndented
        };
}
