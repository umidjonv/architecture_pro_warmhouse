using HomeApi.Models;

namespace HomeApi.Services;

public class HomeService
{
    private readonly List<Home> _homes = new();
    private int _homeIdCounter = 1;
    private readonly SmartHomeClient _smartHomeClient;
    private readonly TemperatureClient _temperatureClient;
    private readonly TelemetryClient _telemetryClient;

    public HomeService(SmartHomeClient smartHomeClient, TemperatureClient temperatureClient, TelemetryClient telemetryClient)
    {
        _smartHomeClient = smartHomeClient;
        _temperatureClient = temperatureClient;
        _telemetryClient = telemetryClient;

        // Add sample home (sensors will be synced from smart_home)
        var sampleHome = new Home
        {
            Id = _homeIdCounter++,
            Name = "My Smart Home",
            Address = "123 Main Street"
        };
        _homes.Add(sampleHome);
    }

    public List<Home> GetAllHomes() => _homes;

    public Home? GetHomeById(int id) => _homes.FirstOrDefault(h => h.Id == id);

    public Home CreateHome(CreateHomeDto dto)
    {
        var home = new Home
        {
            Id = _homeIdCounter++,
            Name = dto.Name,
            Address = dto.Address
        };
        _homes.Add(home);
        return home;
    }

    public bool DeleteHome(int id)
    {
        var home = _homes.FirstOrDefault(h => h.Id == id);
        if (home == null) return false;
        _homes.Remove(home);
        return true;
    }

    public async Task<Sensor?> AddSensorToHomeAsync(int homeId, AddSensorDto dto)
    {
        var home = _homes.FirstOrDefault(h => h.Id == homeId);
        if (home == null) return null;

        // Create sensor in smart_home via HTTP
        var smartHomeSensor = await _smartHomeClient.CreateSensorAsync(new CreateSensorRequest
        {
            Name = dto.Name,
            Type = dto.Type,
            Location = dto.Location,
            Unit = dto.Unit
        });

        if (smartHomeSensor == null) return null;

        var sensor = new Sensor
        {
            Id = smartHomeSensor.Id,
            Name = smartHomeSensor.Name,
            Type = smartHomeSensor.Type,
            Location = smartHomeSensor.Location,
            Unit = smartHomeSensor.Unit,
            Value = smartHomeSensor.Value,
            Status = smartHomeSensor.Status
        };
        home.Sensors.Add(sensor);
        return sensor;
    }

    public async Task<bool> RemoveSensorFromHomeAsync(int homeId, int sensorId)
    {
        var home = _homes.FirstOrDefault(h => h.Id == homeId);
        var sensor = home?.Sensors.FirstOrDefault(s => s.Id == sensorId);
        if (sensor == null) return false;

        // Delete sensor from smart_home via HTTP
        await _smartHomeClient.DeleteSensorAsync(sensorId);

        home!.Sensors.Remove(sensor);
        return true;
    }

    public async Task<Sensor?> UpdateSensorValueAsync(int homeId, int sensorId)
    {
        var home = _homes.FirstOrDefault(h => h.Id == homeId);
        var sensor = home?.Sensors.FirstOrDefault(s => s.Id == sensorId);
        if (sensor == null) return null;

        // Get temperature from temperature-api
        var tempResponse = await _temperatureClient.GetTemperatureAsync(location: sensor.Location);
        if (tempResponse != null)
        {
            sensor.Value = tempResponse.Temperature;
            sensor.Status = "active";

            // Sync value to smart_home
            await _smartHomeClient.UpdateSensorValueAsync(sensorId, tempResponse.Temperature, "active");

            // Send telemetry
            await _telemetryClient.SendTelemetryAsync(new TelemetryData
            {
                SensorId = sensor.Id,
                SensorName = sensor.Name,
                SensorType = sensor.Type,
                Location = sensor.Location,
                Value = tempResponse.Temperature,
                Unit = sensor.Unit,
                Source = "home-api"
            });
        }

        return sensor;
    }

    public async Task<List<TelemetryRecord>?> GetSensorTelemetryAsync(int sensorId, int limit = 100)
    {
        return await _telemetryClient.GetSensorTelemetryAsync(sensorId, limit);
    }

    public async Task<TelemetryStatsResponse?> GetSensorStatsAsync(int sensorId)
    {
        return await _telemetryClient.GetSensorStatsAsync(sensorId);
    }

    public async Task<Home?> SyncSensorsFromSmartHomeAsync(int homeId)
    {
        var home = _homes.FirstOrDefault(h => h.Id == homeId);
        if (home == null) return null;

        var smartHomeSensors = await _smartHomeClient.GetAllSensorsAsync();
        if (smartHomeSensors == null) return home;

        home.Sensors = smartHomeSensors.Select(s => new Sensor
        {
            Id = s.Id,
            Name = s.Name,
            Type = s.Type,
            Location = s.Location,
            Unit = s.Unit,
            Value = s.Value,
            Status = s.Status
        }).ToList();

        return home;
    }
}
