using System.IO;
using System.Text.Json;

namespace ShoppingMall.Client.Services;

public class AppConfiguration
{
    private readonly string _configPath;

    public AppConfiguration()
    {
        var appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ShoppingMall");
        Directory.CreateDirectory(appDir);
        _configPath = Path.Combine(appDir, "config.json");
    }

    public ClientConfig Load()
    {
        if (!File.Exists(_configPath))
            return new ClientConfig();

        var json = File.ReadAllText(_configPath);
        return JsonSerializer.Deserialize<ClientConfig>(json) ?? new ClientConfig();
    }

    public void Save(ClientConfig config)
    {
        var json = JsonSerializer.Serialize(config);
        File.WriteAllText(_configPath, json);
    }
}

public class ClientConfig
{
    public string ServerUrl { get; set; } = "http://localhost:5000";
    public Guid TerminalId { get; set; }
    public Guid StoreId { get; set; }
    public string TerminalName { get; set; } = "POS-1";
    public bool AutoConnect { get; set; } = true;
}
