using System.Text.Json;

namespace HomeApi.Services;

public class TemperatureClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public TemperatureClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<TemperatureResponse?> GetTemperatureAsync(string? location = null, string? sensorId = null)
    {
        var query = new List<string>();
        if (!string.IsNullOrEmpty(location)) query.Add($"location={Uri.EscapeDataString(location)}");
        if (!string.IsNullOrEmpty(sensorId)) query.Add($"sensorId={sensorId}");

        var url = "/temperature" + (query.Any() ? "?" + string.Join("&", query) : "");

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;

        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TemperatureResponse>(responseJson, _jsonOptions);
    }
}

public class TemperatureResponse
{
    public string SensorId { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
