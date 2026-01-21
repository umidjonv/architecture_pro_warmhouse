namespace TelemetryApi.Models;

public class TelemetryRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int SensorId { get; set; }
    public string SensorName { get; set; } = string.Empty;
    public string SensorType { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Source { get; set; } = string.Empty;
}

public class CreateTelemetryDto
{
    public int SensorId { get; set; }
    public string SensorName { get; set; } = string.Empty;
    public string SensorType { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
}

public class TelemetryQuery
{
    public int? SensorId { get; set; }
    public string? Location { get; set; }
    public string? SensorType { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int Limit { get; set; } = 100;
}

public class TelemetryStats
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
