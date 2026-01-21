using System.Text;
using System.Text.Json;

namespace HomeApi.Services;

public class TelemetryClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public TelemetryClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<bool> SendTelemetryAsync(TelemetryData data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/telemetry", content);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<TelemetryRecord>?> GetSensorTelemetryAsync(int sensorId, int limit = 100)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/telemetry/sensor/{sensorId}?limit={limit}");
            if (!response.IsSuccessStatusCode) return null;

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<TelemetryRecord>>(responseJson, _jsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<TelemetryStatsResponse?> GetSensorStatsAsync(int sensorId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/telemetry/sensor/{sensorId}/stats");
            if (!response.IsSuccessStatusCode) return null;

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TelemetryStatsResponse>(responseJson, _jsonOptions);
        }
        catch
        {
            return null;
        }
    }
}

public class TelemetryData
{
    public int SensorId { get; set; }
    public string SensorName { get; set; } = string.Empty;
    public string SensorType { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Source { get; set; } = "home-api";
}

public class TelemetryRecord
{
    public Guid Id { get; set; }
    public int SensorId { get; set; }
    public string SensorName { get; set; } = string.Empty;
    public string SensorType { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Source { get; set; } = string.Empty;
}

public class TelemetryStatsResponse
{
    public int SensorId { get; set; }
    public string Location { get; set; } = string.Empty;
    public double MinValue { get; set; }
    public double MaxValue { get; set; }
    public double AvgValue { get; set; }
    public int Count { get; set; }
    public DateTime FirstReading { get; set; }
    public DateTime LastReading { get; set; }
}
