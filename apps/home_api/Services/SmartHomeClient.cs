using System.Text;
using System.Text.Json;

namespace HomeApi.Services;

public class SmartHomeClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public SmartHomeClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<SensorResponse?> CreateSensorAsync(CreateSensorRequest request)
    {
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/v1/sensors", content);
        if (!response.IsSuccessStatusCode) return null;

        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<SensorResponse>(responseJson, _jsonOptions);
    }

    public async Task<SensorResponse?> GetSensorAsync(int id)
    {
        var response = await _httpClient.GetAsync($"/api/v1/sensors/{id}");
        if (!response.IsSuccessStatusCode) return null;

        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<SensorResponse>(responseJson, _jsonOptions);
    }

    public async Task<List<SensorResponse>?> GetAllSensorsAsync()
    {
        var response = await _httpClient.GetAsync("/api/v1/sensors");
        if (!response.IsSuccessStatusCode) return null;

        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<SensorResponse>>(responseJson, _jsonOptions);
    }

    public async Task<bool> DeleteSensorAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/v1/sensors/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<SensorResponse?> UpdateSensorValueAsync(int id, double value, string status)
    {
        var request = new { value, status };
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PatchAsync($"/api/v1/sensors/{id}/value", content);
        if (!response.IsSuccessStatusCode) return null;

        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<SensorResponse>(responseJson, _jsonOptions);
    }
}

public class CreateSensorRequest
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
}

public class SensorResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public double? Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
