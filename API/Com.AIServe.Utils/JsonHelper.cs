using System.Text.Json;

namespace Com.AIServe.Utils;

public static class JsonHelper
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public static string Serialize(object obj) => JsonSerializer.Serialize(obj, _options);
    public static T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _options);
}
